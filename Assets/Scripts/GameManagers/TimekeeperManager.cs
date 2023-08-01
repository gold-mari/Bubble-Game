using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

// Beat detection logic written with the help of ColinVAudio:
// https://www.youtube.com/watch?v=hNQX1fsQL4Q

public class TimekeeperManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The Endgame Manager present in the scene.")]
    [SerializeField]
    private EndgameManager endgameManager;
    [Tooltip("Not gonna write a meaningful tooltip for this one since it's next on the list to be deprecated.")]
    [SerializeField]
    private float waitLength = 159f;
    [Tooltip("The music event that plays in this scene.")]
    [SerializeField]
    private FMODUnity.EventReference musicEvent;
    
    // ================================================================
    // Internal variables
    // ================================================================

    // A reference to the instance of musicEvent.
    private FMOD.Studio.EventInstance instance;
    // Store a struct to the timeline info that we will reference. We want this struct to
    // be sequential in memory, so add the following tag.
    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        // The current beat in the song. Resets per bar.
        public int currentBeat = 0;
        // FMOD.StringWrapper is an FMOD string type. Look for the last timeline marker.
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }
    // An instance of our timelineInfo class which other scripts will refer to.
    public TimelineInfo timelineInfo;
    // GCHandle is a tool used to help access managed memory in runtime. We're streaming
    // data using C++ here, and use this handle to prevent garbage collection while we're
    // doing it.
    private GCHandle timelineHandle;
    // An event callback variable which ...
    // TODO TODO TODO TODO
    // TODO TODO TODO TODO
    // TODO TODO TODO TODO
    // TODO TODO TODO TODO
    private FMOD.Studio.EVENT_CALLBACK beatCallback;

    
    // ================================================================
    // Default methods
    // ================================================================

    void Awake()
    {
        // Awake is called before Start. We use it to start our music.
        // ================

        // A music event is defined if it has a nonzero path length. Check if the music
        // event is undefined.
        bool eventExists = (musicEvent.Path.Length > 0);
        Debug.Assert( eventExists, "TimekeeperManager Error: Awake() failed: musicEvent must not be null.", this );

        instance = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
        instance.start();
    }

    void Start()
    {
        // Start is called before the first frame update. For now, we just use it to
        // start the music.
        // ================

        // We don't need to check if the music event exists since we do that in Awake.
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

        StartCoroutine(BAD_BAD_BAD_DEBUG_REPLACE_THIS_waitWin());
    }

    void OnDestroy()
    {
        // When this script is destroyed, stop the music and free the GCHandle.
        // ================

        // Reset the instance's data.
        instance.setUserData(IntPtr.Zero);
        // Stop the instance.
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
        // Free the GCHandle.
        timelineHandle.Free();
    }

    void OnGUI()
    {
        // Prints timelineInfo stats to an onscreen GUI box.
        // ================

        GUILayout.Box(String.Format("Current Beat = {0}, Last Marker = {1}", timelineInfo.currentBeat, (string)timelineInfo.lastMarker));
    }

    // ================================================================
    // Data-manipulation methods
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
        Debug.Assert( result == FMOD.RESULT.OK, "TimekeeperManager Error: BeatEventCallback() failed: " + result);
        // If the timelineInfoPtr is not zero, then access data from it.
        if ( timelineInfoPtr != IntPtr.Zero ) {
            // Define timelineHandle as a new GCHandle produced from our timelineInfoPtr,
            // and convert that handle into a TimelineInfo object.
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            // Do different behavior based on what type of callback this function was called with.
            switch ( type ) {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        // Convert parameterPtr to parameter. We use marshalling because
                        // we're going from a pointer to a C# variable.
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        // Set currentBeat to the most recent beat.
                        timelineInfo.currentBeat = parameter.beat;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        // Convert parameterPtr to parameter. We use marshalling because
                        // we're going from a pointer to a C# variable.
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

    // ================================================================
    // Soon-to-be-deprecated methods
    // ================================================================

    IEnumerator BAD_BAD_BAD_DEBUG_REPLACE_THIS_waitWin()
    {
        yield return new WaitForSeconds(waitLength);
        endgameManager.TriggerWin();
    }
}
