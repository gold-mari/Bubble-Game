using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoggerHolder : MonoBehaviour
{
    [SerializeField, Tooltip("The logger tracking our stats.")]
    private StatVarLog statLog;
    [SerializeField, Tooltip("Whether or not we clear the log on start.\n\nDefault: false")]
    private bool clearOnStart = false;

    private void Start()
    {
        if (clearOnStart) statLog.ClearLog();
    }

    public void Log()
    {
        statLog.LogStats(SceneManager.GetActiveScene().name);
    }

    public void CopyToClipboard()
    {
        statLog.CopyLogToClipboard();
    }
}
