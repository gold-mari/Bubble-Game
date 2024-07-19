using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

[CreateAssetMenu(fileName="New Log", menuName="Variables/StatVarLog")]
public class StatVarLog : ScriptableObject
{
    [SerializeField]
    private StatVarLogStat[] stats = new StatVarLogStat[4];

    [SerializeField, ReadOnly]
    private string id = "";
    [SerializeField, ReadOnly, TextArea(8,20)]
    private string log = "";

    public void ClearLog()
    {
        log = "";
        id = $"{Random.Range(0,65536):X4}"; // Set id to a random 4-digit hex string
    }

    public void LogStats(string levelName)
    {
        if (log != "") log += "\n";

        string levelLog = $"{id},{levelName},";
        foreach (StatVarLogStat stat in stats) {
            levelLog += $"{stat.variable.value},";
        }

        // Remove the final comma
        levelLog = levelLog.Remove(levelLog.Length-1);

        log += levelLog;
    }

    public void CopyLogToClipboard()
    {
        // Copys the log to the system clipboard.
        GUIUtility.systemCopyBuffer = "TEST_TEST_TEST\n" + log;
    }
}

[System.Serializable]
public class StatVarLogStat
{
    public string ID;
    public uintVar variable;
}

// [CustomEditor(typeof(StatVarLog))]
// public class StatVarLogEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();

//         var statVarLog = target as StatVarLog;
        
//         if(GUILayout.Button("Clear Log")) {
//             statVarLog.ClearLog();
//         }

//         if(GUILayout.Button("Log Current Values")) {
//             statVarLog.LogStats("editor");
//         }

//         if(GUILayout.Button("Copy Log To Clipboard")) {
//             statVarLog.CopyLogToClipboard();
//         }
//     }
// }
