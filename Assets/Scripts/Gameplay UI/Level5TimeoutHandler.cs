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
    [SerializeField, Tooltip("A UnityEvent called when the timeout starts.")]
    private UnityEvent OnStartTimeout;
    [SerializeField, Tooltip("A UnityEvent called when the timeout ends.")]
    private UnityEvent OnEndTimeout;

    protected new void Start()
    {
        base.Start();
        musicManager.SetParameter(endTimeoutParameter, 0);
    }

    protected override void OnSwitchMap(string mapName)
    {
        if (mapName == timeoutTag) {
            print($"Level5TimeoutHandler: Encountered timeout tag (\"{timeoutTag}\")");
            OnStartTimeout?.Invoke();

            StartCoroutine(DEBUG_WAITENDTIMEOUT());
        }
    }

    public void EndTimeout()
    {
        musicManager.SetParameter(endTimeoutParameter, 1);

        OnStartTimeout?.Invoke();
    }

    private IEnumerator DEBUG_WAITENDTIMEOUT()
    {
        yield return new WaitForSeconds(6);
        EndTimeout();
    }
}
