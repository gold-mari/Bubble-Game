using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueLine() {}
    public DialogueLine(string ourActor, string ourActions, string ourText)
    {
        actor = ourActor;
        actions = ourActions;
        text = ourText;
    }

    public string actor;
    public string actions;
    public string text;
}

public class DialogueHandler : MonoBehaviour
{
    public TextAsset dialogueFile;
    public TMP_Text textbox;
    public TMP_Text speaker;

    int index;
    Dictionary<int, DialogueLine> lineDict = new();

    // Start is called before the first frame update
    void Start()
    {
        PopulateLineDict();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (index < lineDict.Count-1)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            UpdateText();
        }
    }

    void PopulateLineDict()
    {
        Dialogue dialogue = JsonUtility.FromJson<Dialogue>(dialogueFile.text);

        for (int i = 0; i < dialogue.lines.Count; i++)
        {
            print(i);
            // Create a new ScriptLine object, and then populate it.
            lineDict.Add(i, dialogue.lines[i]);
        }

        textbox.text = lineDict[index].text;
    }

    void UpdateText()
    {
        textbox.text = lineDict[index].text;
        speaker.text = lineDict[index].actor;
    }
}
