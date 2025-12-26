using NaughtyAttributes;
using UnityEngine;
using TMPro;

public class lyricDisplay : MonoBehaviour
{
    public string[] lines;
    public TMP_Text textBox;

    public string CurrentLine => (index >= 0 && index < lines.Length) ? lines[index] : $"INDEX ERROR: {index} OUT OF BOUNDS";

    private int index = 0;

    [Button]
    public void NextLine()
    {
        if (index < 0 || index >= lines.Length) 
            index = 0;
        else 
            index++;

        textBox.text = CurrentLine;
    }
}