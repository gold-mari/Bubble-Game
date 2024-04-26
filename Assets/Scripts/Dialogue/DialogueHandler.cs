using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The JSON file holding our script as a serialized Dialogue object.")]
    private TextAsset dialogueFile;
    [SerializeField, Tooltip("A list of all actors involved in Dialogues used on this component.")]
    private Actor[] actors;
    [SerializeField, Tooltip("The text object which displays our main text.")]
    private TMP_Text textbox;
    [SerializeField, Tooltip("The text object which displays the name of our speaker.")]
    private TMP_Text speaker;
    [SerializeField, Tooltip("The base delay, in seconds, between letters appearing in our typewriter effect.\n\nDefault: 0.025")]
    float typewriterDelay = 0.025f;
    [SerializeField, Tooltip("The delay, in seconds, after a textbox finishes animating before we can advance.\n\nDefault: 0.1")]
    float advanceDelay = 0.5f;
    [SerializeField, Tooltip("Whether this text object should begin displaying text on Start()\n\nDefault: true")]
    bool beginInStart = true;

    [SerializeField, Tooltip("An action invoked when advancing from the final line.")]
    public UnityEvent ActionOnEnd;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The DialogueLine we're currently displaying.
    int index;
    // The dictionary of lines, accessed by index.
    Dictionary<int, DialogueLine> lineDict = new();
    // The dictionary of actors, accessed by name.
    Dictionary<string, DialogueActor> actorDict = new();
    // A timer we use to countdown when we can display a new character in our text, as per a typewriter effect.
    float typewriterTimer;
    // A timer we use that begins counting once a textbox is fully displayed. Once it's up, we can advance.
    // We do this to provide a buffer for short textboxes, so that they aren't accidentally skipped if the
    // player is impatient and is bypassing the typewriter effect.
    float advanceTimer;
    // Whether we have started displaying text.
    bool started = false;
    // Whether a line has finished animating.
    bool lineFinished = false;
    // Whether we can advance to the next textbox.
    bool canAdvance = false;

    // ==============================================================
    // Initialization methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to initialize dictionaries and
        // change to our initial text.
        // ================

        PopulateActorDict();
        PopulateLineDict();
        if (beginInStart) Begin();
    }

    private void PopulateActorDict()
    {
        // Adds our actors to our dictionary, so they can be accessed quickly.
        // ================

        foreach (Actor actor in actors)
        {
            actorDict.Add(actor.actorName, actor.actorObject);
        }
    }

    void PopulateLineDict()
    {
        // Parses our JSON file and adds our DialogueLines to our dictionary, so they can be accessed quickly.
        // ================

        Dialogue dialogue = JsonUtility.FromJson<Dialogue>(dialogueFile.text);

        for (int i = 0; i < dialogue.lines.Count; i++)
        {
            lineDict.Add(i, dialogue.lines[i]);
        }
    }

    public void Begin()
    {
        started = true;
        ChangeText(0);
    }

    // ==============================================================
    // Continuous methods
    // ==============================================================

    void Update()
    {
        // Update is called once per frame. We use it to detect input and direct both changing text,
        // and updating text fill.
        // ================

        if (!started || Time.timeScale == 0) return;

        // On input...
        if (Input.GetButtonDown("Fire1"))
        {
            // If we can, advance to the next line, and reset all our timers.
            if (canAdvance)
            {
                if (index < lineDict.Count-1)
                {
                    index++;
                }
                else
                {
                    ActionOnEnd?.Invoke();
                    return;
                }

                lineFinished = false;
                canAdvance = false;
                typewriterTimer = 0;
                advanceTimer = 0;
                ChangeText(index);
            }
            // If the line is not finished, finish it.
            else if (!lineFinished)
            {
                UpdateTextFill(index, impatient:true);
            }
        }

        // If the line is not finished, then advance it once our timer is up.
        if (!lineFinished)
        {
            if (typewriterTimer > typewriterDelay)
            {
                typewriterTimer = 0;
                UpdateTextFill(index);
            }
            else
            {
                typewriterTimer += Time.deltaTime;
            }
        }
        // If it is, start ticking down our advance timer.
        else
        {
            if (!canAdvance && advanceTimer > advanceDelay)
            {
                //print("Can advance!");
                advanceTimer = 0;
                canAdvance = true;
            }
            else
            {
                advanceTimer += Time.deltaTime;
            }
        }
    }

    // ==============================================================
    // Data-handling methods
    // ==============================================================

    public void NextLine()
    {
        index++;

        lineFinished = false;
        canAdvance = false;
        typewriterTimer = 0;
        advanceTimer = 0;
        ChangeText(index);
    }

    void ChangeText(int line)
    {
        // Changes the line of text to be displayed, along with associated actions and actor names.
        // ================

        textbox.maxVisibleCharacters = 0;
        textbox.text = lineDict[line].text;
        //print($"New line: {textbox.text}");
        
        speaker.text = lineDict[line].actor;

        if (lineDict[line].actions != "")
        {
            string[] actions = lineDict[line].actions.Split(',');
            foreach (string action in actions)
            {
                // Split each action into 2 tokens: the actor, and the trigger. We use Trim to trim whitespace.
                string[] tokens = action.Trim().Split('.');
                Debug.Assert(tokens.Length == 2, $"DialogueHandler error: UpdateText failed. Token count of action '{action}' " +
                                                $"on line {line} was not 2: {tokens.Length}");
                Debug.Assert(actorDict.ContainsKey(tokens[0]), $"DialogueHandler error: UpdateText failed. First token of " +
                                                            $"action '{action}' on line {line} was not a valid actor: {tokens[0]}");
                actorDict[tokens[0].Trim()].Trigger(tokens[1]);
            }
        }
    }

    void UpdateTextFill(int line, bool impatient=false)
    {
        // Updates the text fill- making more of the text visible over repeated calls, like a typewriter.
        // If this is called with impatient = true, then all of the text immediately becomes visible.
        // ================

        if (impatient)
        {
            textbox.maxVisibleCharacters = textbox.textInfo.characterCount;
            return;
        }

        if (textbox.maxVisibleCharacters < textbox.textInfo.characterCount)
        {
            textbox.maxVisibleCharacters++;
        }
        else // if the line is full...
        {
            //print("Line finished!");
            lineFinished = true;
        }
    }
}

// ==============================================================
// Helper classes
// ==============================================================

[System.Serializable]
public class Dialogue
{
    [Tooltip("The list of DialogueLines which store script data.")]
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    [Tooltip("The string ID of the actor speaking right now. Is also displayed in the 'speaker' textbox.")]
    public string actor;
    [Tooltip("Triggers to send to our actors' DialogueActor component.\n\n"
            +"Must be of the format 'actor1.TriggerName,actor2.TriggerName' etc.")]
    public string actions;
    [Tooltip("The text to display in the main textbox.")]
    public string text;
}

[System.Serializable]
public class Actor
{
    [Tooltip("The string ID of this actor. Must be aligned with the actor string in DialogueLine.")]
    public string actorName;
    [Tooltip("The DialogueActor associated with this actor. This component will recieve triggers for this actor.")]
    public DialogueActor actorObject;
}