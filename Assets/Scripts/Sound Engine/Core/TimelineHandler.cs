using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

// Beat detection logic written with the help of ColinVAudio:
// https://www.youtube.com/watch?v=hNQX1fsQL4Q
// Partial subdivision, chiefly the idea of using ChannelGroups for DSP clock counting from user
// bloo_regard_q_kazoo on the FMOD forums.
// https://drive.google.com/file/d/1r8ROjgsMh-mwKqGTZT7IWMCsJcs3GuU9/view AND
// https://qa.fmod.com/t/perfect-beat-tracking-in-unity/18788/2

public class TimelineHandler
{
    // ================================================================
    // Internal variables
    // ================================================================

    // A reference to the instance of musicEvent.
    private FMOD.Studio.EventInstance instance;
    // Whether or not we should update.
    private bool shouldUpdate = true;

    // timeline callbacks: ============================================

    // ================================== //
    // PUBLIC FIELDS                      //
    // ================================== //

    // Store a struct to the timeline info that we will reference. We want this struct to be 
    // sequential in memory, so add the following tag.
    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        // The current beat in the song. Resets per bar.
        public int currentBeat = 0;
        // The current position in the song, in milliseconds.
        public int currentPositionMS = 0;
        // The current position in the song, in seconds.
        public double currentPositionS = 0;
        // The current tempo in the song, in beats per minute.
        public float currentTempo = 0;
        // FMOD.StringWrapper is an FMOD string type. Look for the last timeline marker.
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }
    // An instance of our timelineInfo class which other scripts will refer to.
    public TimelineInfo timelineInfo { get; private set; }
    // Actions shouted each beat, when the tempo updates, and at a new marker, respectively.
    public System.Action beatUpdated, tempoUpdated;
    public System.Action<string> markerUpdated;

    // Internal fields

    // GCHandle is a tool used to help access managed memory in runtime. We're streaming data using
    // C here, and use this handle to prevent garbage collection while we're doing it.
    private GCHandle timelineHandle;
    // An event callback variable which we assign to the callback function BeatEventCallback.
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    // The previous values (as of the last frame) of currentBeat.
    private int lastBeat = 0;
    // The same for currentTempo.
    private float lastTempo = 0f;
    // The same for lastMarker.
    private string lastLastMarker = "";

    // duration: ======================================================

    // ================================== //
    // PUBLIC FIELDS                      //
    // ================================== //

    // The length of musicEvent in seconds.
    public float musicLength { get; private set; }
    // The difference between rawLastTime and rawCurrentTime at any point. Effectively, the precise
    // amount of time that passes per frame.
    public double DSPdeltaTime { get; private set; }
    // The accumulated total DSPdeltaTime while instanceRunning has been true.
    public double DSPTime { get; private set; }

    // Internal fields

    // A ChannelGroup used to access the DSP clock, which runs at sample rate.
    private FMOD.ChannelGroup masterChannelGroup;
    // The sample rate of our master channel group.
    private int sampleRate;
    // An unsigned long which tracks our current number of samples.
    private ulong dspClock;
    // A reference to the DSP clock of the parent ChannelGroup. Discarded.
    private ulong DISCARD_parentDSP;
    // The current and previous DSP times, in seconds.
    private double rawLastTime = 0f, rawCurrentTime = 0f;
    // If our event instance is actively running.
    bool accumulateDSPTime = false;

    // subdivision: ===================================================

    // ================================== //
    // PUBLIC FIELDS                      //
    // ================================== //

    // Doubles used to hold the length, in seconds, of different note values at the current tempo.
    public double length4th { get; private set; }
    public double length8th { get; private set; }
    public double length16th { get; private set; }
    public double length32nd { get; private set; }
    // Actions shouted each eighth note, each sixteenth note, and each thirtysecond note.
    public System.Action eighthNoteEvent, sixteenthNoteEvent, thirtysecondNoteEvent;

    // Internal fields

    // Timers used to help determine if we ought to fire the note events yet at a given time.
    private double timer8th = 0, timer16th = 0, timer32nd = 0;
    // Timers used to determine if we ought to fire the note events yet at a given time.
    private bool fire8th = false, fire16th = false, fire32nd = false;
    
    // ================================================================
    // Initializers and finalizers
    // ================================================================

    public TimelineHandler(FMOD.Studio.EventInstance eventInstance)
    {
        // Constructor.
        // ================

        instance = eventInstance;

        // ================================
        // Part 1: Get duration!
        // ================================
        FMOD.Studio.EventDescription description;
        instance.getDescription(out description);
        // Define the length of the song in seconds.
        int lengthInMilliseconds = 0;
        description.getLength(out lengthInMilliseconds);
        musicLength = lengthInMilliseconds/1000f;

        // ================================
        // Part 2: Get callbacks!
        // ================================
        // Instantiate our timeline info and beat callback.
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        // Assign our timelineHandle to space allocated from our timelineInfo. We pin it
        // using GCHandleType.Pinned so it isn't garbage collected.
        // This will read data from timelineHandle into timelineInfo.
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        // Assign timelineHandle to our music event. This tells it to read data through
        // timelineHandle (and thus into timelineInfo.)
        instance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        // Set the callbacks on our instance. Look for beat and marker callbacks.
        instance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | 
                                           FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        // Create our master channel group for monitoring subdivisions.
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);
        // Define our sample rate. Discard the outs for the speaker mode and the number of speakers.
        FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out sampleRate,
                                                              out FMOD.SPEAKERMODE DISCARD_mode,
                                                              out int DISCARD_num);

        // ================================
        // Part 3: Calculate subdivisions!
        // ================================
        fire8th = fire16th = fire32nd = true;
    }

    ~TimelineHandler()
    {
        // Finalizer.
        // When this object is destroyed, stop the music and free the GCHandle.
        // ================

        // Reset the instance's data.
        instance.setUserData(IntPtr.Zero);
        // Stop the instance.
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
        // Free the GCHandle.
        timelineHandle.Free();
    }

    // ================================================================
    // Continuous methods
    // ================================================================

    public void Update()
    {
        // Update is called once per frame from MusicManager. We use it to do the following:
        // - Shout beatUpdated when timelineInfo.currentBeat changes
        // - Shout tempoUpdated when timelineInfo.currentTempo changes
        // - Shout markerUpdated when timelineInfo.lastMarker changes
        // - Update DSP time
        // - Check subdivision events
        // ================

        if (!shouldUpdate)
        {
            return;
        }

        // Update our DSPTime.
        UpdateDSPTime();

        if (lastTempo != timelineInfo.currentTempo) {
            lastTempo = timelineInfo.currentTempo;
            tempoUpdated?.Invoke();
        }
        if (lastLastMarker != timelineInfo.lastMarker) {
            lastLastMarker = timelineInfo.lastMarker;
            
            // Markers may be packed with multiple tags. Parse them out and update for each one.
            string[] submarkers = ((string)timelineInfo.lastMarker).Split(',');
            foreach (string submarker in submarkers)
            {
                markerUpdated?.Invoke(submarker);
            }
        }
        if (lastBeat != timelineInfo.currentBeat) {
            lastBeat = timelineInfo.currentBeat;
            beatUpdated?.Invoke();

            // Whenever we get a timeline beat callback, correct our DSP time as well.
            CorrectDSPTime();
            // Also, note that we should fire all our subdivision events to avoid drift.
            fire8th = fire16th = fire32nd = true;
        }

        // Also calculate subdivisions.
        ShoutSubdivisions();
    }

    public void StopUpdating()
    {
        // Stops this timeline handler from updating. 
        // 
        // !!! IMPORTANT !!!
        // This should ONLY be used when the instance we're handling is ABOUT TO END.
        // !!! IMPORTANT !!!
        // ================

        shouldUpdate = false;
    }

    public void OnGUI()
    {
        // Prints timelineInfo stats to an onscreen GUI box.
        // ================

        GUILayout.Box(String.Format("Current beat = {0}, Last marker = {1}", timelineInfo.currentBeat, (string)timelineInfo.lastMarker));
        GUILayout.Box(String.Format("Current position = {0}, Current BPM = {1}", timelineInfo.currentPositionMS, timelineInfo.currentTempo));
        GUILayout.Box(String.Format("Current time = {0:0.0000000000}", DSPTime));
        GUILayout.Box(String.Format("Song length = {0} seconds", musicLength));
    }

    // ================================================================
    // DSP-Time methods
    // ================================================================

    public void StartDSPClock(bool instanceStarted)
    {
        // DSP time runs whether or not the FMOD event is playing. In order to ensure
        // rawCurrentTime produces an accurate output, we call StartDSPClock when 
        // we start our event instance, or when the game plays.
        //
        // Passes in if our instance has already started playing. If it hasn't, do not
        // accumulate time, even if we're told to start the clock.
        // ================

        if (!instanceStarted)
        {
            return;
        }

        accumulateDSPTime = true;
        masterChannelGroup.setPaused(false);
    }

    public void StopDSPClock()
    {
        // DSP time runs whether or not the FMOD event is playing. In order to ensure
        // rawCurrentTime produces an accurate output, we call StopDSPClock when 
        // we stop our event instance, or when the game is paused.
        // ================

        accumulateDSPTime = false;
        masterChannelGroup.setPaused(true);
    }

    public void ResetDSPClock()
    {
        // DSP time runs whether or not the FMOD event is playing. In order to ensure
        // rawCurrentTime produces an accurate output, we call ResetDSPClock when 
        // we reset our DSPTime.
        // ====

        DSPTime = 0;
    }

    void UpdateDSPTime()
    {
        // Based on the value of dspClock from the masterChannelGroup, writes the 
        // DSPdeltaTime since the last update and accumulates DSPTime.
        // Shoutouts to bloo_regard_q_kazoo from the FMOD forums!
        // ================

        // Write into dspClock.
        masterChannelGroup.getDSPClock(out dspClock, out DISCARD_parentDSP);
        // Cache the rawCurrentTime as rawLastTime, and update rawCurrentTime.
        rawLastTime = rawCurrentTime;

        rawCurrentTime = (double)dspClock / (double)sampleRate;
        // Calculate the deltaTime between these values.
        DSPdeltaTime = rawCurrentTime - rawLastTime;

        // If we should, accumulate DSPTime.
        if (accumulateDSPTime)
        {
            DSPTime += DSPdeltaTime;
        }
    }

    private void CorrectDSPTime()
    {
        // Latency builds up in DSPTime when the editor pauses and plays. To ensure that
        // DSPTime is as accurate as possible, whenever we hit a beat event, set DSPTime
        // to exactly the current timeline position.
        // ================

        DSPTime = timelineInfo.currentPositionS;
    }

    // ================================================================
    // Subdivision methods
    // ================================================================

    void CalculateSubdivisionLengths()
    {
        // Calculates the length of subdivisions.
        // ================

        // First, convert tempo to seconds per beat.
        length4th = Mathf.Pow((timelineInfo.currentTempo/60f), -1);
        // Get subdivision lengths.
        length8th = length4th/2f;
        length16th = length8th/2f;
        length32nd = length16th/2f;
    }

    void ShoutSubdivisions()
    {
        // This function calls our subdivision events. Our subdivision events are:
        // - eighthNoteUpdated, called every 8th note
        // - sixteenthNoteUpdated, called every 16th note
        // - thirtysecondNoteUpdated,called every 32nd note
        // ================

        // Define our note length values. We must do this each cycle in case tempo changes.
        CalculateSubdivisionLengths();

        // Check if we should fire any of our subdivision events.
        if (fire8th) {
            // If we're to fire the 8th note event, do so.
            eighthNoteEvent?.Invoke();
            // Reset this timer, and tell all our lower subdivisions to also fire, to
            // prevent drift.
            fire8th = false;
            timer8th = 0;

            fire16th = fire32nd = true;
        }
        if (fire16th) {
            sixteenthNoteEvent?.Invoke();
            // Reset this timer, and tell all our lower subdivisions to also fire, to
            // prevent drift.
            fire16th = false;
            timer16th = 0;

            fire32nd = true;
        }
        if (fire32nd) {
            thirtysecondNoteEvent?.Invoke();
            // Reset this timer.
            fire32nd = false;
            timer32nd = 0;
        }
        
        // Next, update the timers with the DSPdeltaTime, used as a very accurate timer.
        timer8th += DSPdeltaTime;
        timer16th += DSPdeltaTime;
        timer32nd += DSPdeltaTime;

        // Finally, check if we should fire any events on the next update.
        if (timer8th >= length8th) {
            fire8th = fire16th = fire32nd = true;
        }
        else if (timer16th >= length16th) {
            fire16th = fire32nd = true;
        }
        else if (timer32nd >= length32nd) {
            fire32nd = true;
        }
    }

    // ================================================================
    // The Callback Method
    // ================================================================

    // Use this tag to get data from unmanaged memory, which we neeed since we're working
    // with pointers.
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        // A function for processing callback info. instancePtr is a reference to our
        // event instance, type is a reference to what kind of callback we're getting, 
        // and parameterPtr references that data in that callback.
        // ================

        // Convert the instancePtr into an event instance.
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);
        // Using getUserData, get a result from our instance and save it into
        // timelineInfoPtr. result reports if we got expected output from our instance,
        // while timeline info is saved into our timelineInfoPtr.
        // timelineInfoPtr stores the timelineInfo and timelineHandle references we
        // placed into it in Start().
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        // Report an error if result is unexpected.
        Debug.Assert(result == FMOD.RESULT.OK, "TimekeeperManager Error: BeatEventCallback() failed: " + result);
        // If the timelineInfoPtr is not zero, then access data from it.
        if (timelineInfoPtr != IntPtr.Zero) {
            // Define timelineHandle as a new GCHandle produced from our timelineInfoPtr,
            // and convert that handle into a TimelineInfo object.
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            // Do different behavior based on what type of callback this function was called with.
            switch (type) {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        // Convert parameterPtr to parameter. We use marshalling because
                        // we're going from a C pointer to a C# variable.
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        // Set currentBeat, currentPosition, and tempo.
                        timelineInfo.currentBeat = parameter.beat;
                        timelineInfo.currentPositionMS = parameter.position;
                        timelineInfo.currentPositionS = parameter.position/1000f;
                        timelineInfo.currentTempo = parameter.tempo;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        // Convert parameterPtr to parameter. We use marshalling because
                        // we're going from a C pointer to a C# variable.
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        // Set lastMarker to the name of this marker.
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }

        // If we get here, then the event callback was successful.
        return FMOD.RESULT.OK;
    }
}