using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    public string actor;
    public string actions;
    public string text;
}

[System.Serializable]
public class Actor
{
    public string actorName;
    public DialogueActor actorObject;
}

public class DialogueHandler : MonoBehaviour
{
    public TextAsset dialogueFile;
    public Actor[] actors;
    public TMP_Text textbox;
    public TMP_Text speaker;

    int index;
    Dictionary<int, DialogueLine> lineDict = new();
    Dictionary<string, DialogueActor> actorDict = new();

    // Start is called before the first frame update
    void Start()
    {
        PopulateActorDict();
        PopulateLineDict();
        UpdateText(index);
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

            UpdateText(index);
        }
    }

    private void PopulateActorDict()
    {
        foreach (Actor actor in actors)
        {
            actorDict.Add(actor.actorName, actor.actorObject);
        }
    }

    void PopulateLineDict()
    {
        Dialogue dialogue = JsonUtility.FromJson<Dialogue>(dialogueFile.text);

        for (int i = 0; i < dialogue.lines.Count; i++)
        {
            lineDict.Add(i, dialogue.lines[i]);
        }
    }

    void UpdateText(int line)
    {
        textbox.text = lineDict[line].text;
        speaker.text = lineDict[line].actor;

        string[] actions = lineDict[line].actions.Split(',');
        foreach (string action in actions)
        {
            // Split each action into 2 tokens: the actor, and the trigger.
            string[] tokens = action.Split('.');
            Debug.Assert(tokens.Length == 2, $"DialogueHandler error: UpdateText failed. Token count of action '{action}' " +
                                             $"on line {line} was not 2: {tokens.Length}");
            Debug.Assert(actorDict.ContainsKey(tokens[0]), $"DialogueHandler error: UpdateText failed. First token of " +
                                                           $"action '{action}' on line {line} was not a valid actor: {tokens[0]}");
            actorDict[tokens[0]].Trigger(tokens[1]);
        }
    }
}
