using UnityEngine;

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

        chunks[i] = currentChunk[..pos] + $",        {extraActions}{term}" + currentChunk[(pos + term.Length)..];

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