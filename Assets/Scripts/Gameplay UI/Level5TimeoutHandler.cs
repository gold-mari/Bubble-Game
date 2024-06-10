using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

public class Level5TimeoutHandler : ActionOnSwitchMap
{
    [SerializeField, Tooltip("The timeout tag we look for.\n\nDefault: \"timeout\"")]
    private string timeoutTag = "timeout";
    [SerializeField, Tooltip("The timeout tag we look for.\n\nDefault: \"timeout\"")]
    private string endTimeoutParameter = "endTimeout";
    [SerializeField, Tooltip("The event bank run on starting the timeout.")]
    private TimeoutEventBank OnStartTimeout;
    [SerializeField, Tooltip("The event bank run on ending the timeout.")]
    private TimeoutEventBank OnEndTimeout;

    protected new void Start()
    {
        base.Start();
        musicManager.SetParameter(endTimeoutParameter, 0);
    }

    private void Update()
    {
        // Update is called once per frame. We use it to update our eventBanks and by extension the
        // timeout events.
        // ================

        OnStartTimeout.Update();
        OnEndTimeout.Update();
    }

    protected override void OnSwitchMap(string mapName)
    {
        if (mapName == timeoutTag) {
            print($"Level5TimeoutHandler: Encountered timeout tag (\"{timeoutTag}\")");
            OnStartTimeout.Run();
        }
    }

    public void EndTimeout()
    {
        musicManager.SetParameter(endTimeoutParameter, 1);
        OnEndTimeout.Run();
    }
}

// ================================================================
// Helper classes
// ================================================================

[System.Serializable]
public class TimeoutEvent : UnityEvent<Level5TimeoutHandler> {}

[System.Serializable]
public class DelayedTimeoutEvent
{
    [SerializeField]
    private string label;
    [Tooltip("The amount of time after Invoke() is called before the event is invoked.")]
    public float delay;
    [Tooltip("The event to invoke.")]
    public TimeoutEvent ourEvent;

    // The endgame manager associated with this event.
    Level5TimeoutHandler timeoutHandler = null;
    // A list of floats representing queued invocations of our event.
    private List<float> invokeQueue = new();

    public void Invoke(Level5TimeoutHandler handler=null)
    {
        // Adds an invocation to the end of our queue. Takes an optional EndgameManager argument, 
        // acting as a lazy initializer.
        // ====================

        if(timeoutHandler = null) timeoutHandler = handler;
        invokeQueue.Add(delay);
    }

    public void Update()
    {
        // Updates the timers in our invoke queue.
        // IMPORTANT: MUST BE CALLED IN MONOBEHAVIOUR.UPDATE().
        // ================

        if (invokeQueue.Count > 0) {
            for (int i = 0; i < invokeQueue.Count; i++) 
            {
               invokeQueue[i] -= Time.deltaTime;
            }
            while (invokeQueue.Count > 0 && invokeQueue[0] <= 0)
            {
                ourEvent?.Invoke(timeoutHandler);
                invokeQueue.RemoveAt(0);
            }
        }
    }
}

[System.Serializable]
public class TimeoutEventBank
{
    [SerializeField, Tooltip("Events called within this bank. If this is not the last bank, at least one of these " +
                             "events should call EndgameEventCallback.")]
    DelayedTimeoutEvent[] events;

    public void Run()
    {
        // Runs all the events in our bank.
        // ================

        foreach (DelayedTimeoutEvent e in events)
        {
            e.Invoke();
        }   
    }

    public void Update()
    {
        foreach (DelayedTimeoutEvent e in events)
        {
            e.Update();
        }
    }
}
