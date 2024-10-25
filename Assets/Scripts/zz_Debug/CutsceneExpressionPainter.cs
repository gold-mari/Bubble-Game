using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using System.Linq;

public class CutsceneExpressionPainter : MonoBehaviour
{
    // Used to help record expressions for cutscenes.

    public enum Ch { Hannah, Hana, Sammy, Knight, Ghost, Jester, Brute };
    public enum Ex { Neutral, Good, Bad };

    public DialogueHandler dialogueHandler;
    private string fileText;
    private string[] chunks;
    private string currentChunk;
    private int i = 0;

    public void Awake()
    {
        fileText = dialogueHandler.dialogueFile.text;
        chunks = fileText.Split("},");

        dialogueHandler.OnAdvance += UpdateCurrentChunk;
    }

    public void UpdateCurrentChunk(int line)
    {
        i = line;
        currentChunk = chunks[i];
    }

    public void LogExpression(string charL, Ex exprL, string charR, Ex exprR)
    {
        string extraActions = $"{charL}.{exprL},{charR}.{exprR}";

        string term = "\",";

        int pos = currentChunk.IndexOf(term); // Find the first instance (after actor)
        pos = currentChunk.IndexOf(term, pos+1); // Find the second instance (after actions)

        chunks[i] = currentChunk[..pos] + $",\t{extraActions}{term}" + currentChunk[(pos + term.Length)..];

        CopyToClipboard();
    }

    public void CopyToClipboard()
    {
        string reconstructed = string.Join("},\n\t\t\t", chunks);
        GUIUtility.systemCopyBuffer = reconstructed;
    }

    public string GetCurrentChunk()
    {
        return currentChunk;
    }
}

// ================================

[CustomEditor(typeof(CutsceneExpressionPainter))]
public class CutsceneExpressionPainterEditor : Editor
{
    public CutsceneExpressionPainter painter;
    CutsceneExpressionPainter.Ch leftChar = CutsceneExpressionPainter.Ch.Hannah;
    CutsceneExpressionPainter.Ch rightChar = CutsceneExpressionPainter.Ch.Sammy;
    CutsceneExpressionPainter.Ex leftExpr = CutsceneExpressionPainter.Ex.Neutral;
    CutsceneExpressionPainter.Ex rightExpr = CutsceneExpressionPainter.Ex.Neutral;

    private void OnEnable()
    {
        painter = (CutsceneExpressionPainter)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        leftChar = SelectCharacter(leftChar);
        leftExpr = SelectExpression(leftExpr);

        GUILayout.Label($"left ==== {leftChar}.{leftExpr}");

        GUILayout.Space(8);

        rightChar = SelectCharacter(rightChar);
        rightExpr = SelectExpression(rightExpr);

        GUILayout.Label($"right ==== {rightChar}.{rightExpr}");

        GUILayout.Space(16);

        // if (GUILayout.Button("Initialize")) painter.Start();

        if (GUILayout.Button("Submit")) {
            string charL = (leftChar == CutsceneExpressionPainter.Ch.Hana) ? "Ha'na" : leftChar.ToString();
            string charR = (rightChar == CutsceneExpressionPainter.Ch.Hana) ? "Ha'na" : rightChar.ToString();

            painter.LogExpression(charL, leftExpr, charR, rightExpr);
        }

        GUILayout.Space(16);

        GUILayout.Label($"{painter.GetCurrentChunk()}");
    }

    public CutsceneExpressionPainter.Ch SelectCharacter(CutsceneExpressionPainter.Ch original)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Hannah")) return CutsceneExpressionPainter.Ch.Hannah;
        if (GUILayout.Button("Hana")) return CutsceneExpressionPainter.Ch.Hana;
        if (GUILayout.Button("Sammy")) return CutsceneExpressionPainter.Ch.Sammy;
        if (GUILayout.Button("Knight")) return CutsceneExpressionPainter.Ch.Knight;
        if (GUILayout.Button("Ghost")) return CutsceneExpressionPainter.Ch.Ghost;
        if (GUILayout.Button("Jester")) return CutsceneExpressionPainter.Ch.Jester;
        if (GUILayout.Button("Brute")) return CutsceneExpressionPainter.Ch.Brute;
        GUILayout.EndHorizontal();

        return original;
    }

    public CutsceneExpressionPainter.Ex SelectExpression(CutsceneExpressionPainter.Ex original)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Neutral"))
        {
            return CutsceneExpressionPainter.Ex.Neutral;
        }
        if (GUILayout.Button("Good"))
        {
            return CutsceneExpressionPainter.Ex.Good;
        }
        if (GUILayout.Button("Bad"))
        {
            return CutsceneExpressionPainter.Ex.Bad;
        }
        GUILayout.EndHorizontal();

        return original;
    }

}
