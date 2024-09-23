using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatVarLog))]
public class StatVarLogEditor : Editor
{
    StatVarLog log;

    // ================================================================
    // Primary GUI methods
    // ================================================================

    private void OnEnable()
    {
        log = (StatVarLog)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clear")) log.ClearLog();
    }
}
