using UnityEditor;
using UnityEngine;

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

        leftChar = SelectCharacter(leftChar, leftExpr);
        leftExpr = SelectExpression(leftExpr, leftChar);

        GUILayout.Label($"left ==== {leftChar}.{leftExpr}");

        GUILayout.Space(8);

        rightChar = SelectCharacter(rightChar, rightExpr);
        rightExpr = SelectExpression(rightExpr, rightChar);

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

    public CutsceneExpressionPainter.Ch SelectCharacter(
        CutsceneExpressionPainter.Ch original,
        CutsceneExpressionPainter.Ex expression)
    {
        var temp = original;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Hannah")) temp = CutsceneExpressionPainter.Ch.Hannah;
        if (GUILayout.Button("Hana")) temp = CutsceneExpressionPainter.Ch.Hana;
        if (GUILayout.Button("Sammy")) temp = CutsceneExpressionPainter.Ch.Sammy;
        if (GUILayout.Button("Knight")) temp = CutsceneExpressionPainter.Ch.Knight;
        if (GUILayout.Button("Ghost")) temp = CutsceneExpressionPainter.Ch.Ghost;
        if (GUILayout.Button("Jester")) temp = CutsceneExpressionPainter.Ch.Jester;
        if (GUILayout.Button("Brute")) temp = CutsceneExpressionPainter.Ch.Brute;
        GUILayout.EndHorizontal();

        if (temp != original) {
            painter.dialogueHandler.TestExpression(temp, expression);
        }

        return temp;
    }

    public CutsceneExpressionPainter.Ex SelectExpression(
        CutsceneExpressionPainter.Ex original,
        CutsceneExpressionPainter.Ch character)
    {
        var temp = original;
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Neutral")) temp = CutsceneExpressionPainter.Ex.Neutral;
        if (GUILayout.Button("Good")) temp = CutsceneExpressionPainter.Ex.Good;
        if (GUILayout.Button("Bad")) temp = CutsceneExpressionPainter.Ex.Bad;
        GUILayout.EndHorizontal();

        if (temp != original) {
            painter.dialogueHandler.TestExpression(character, temp);
        }

        return temp;
    }

}
