using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnEvent : MonoBehaviour
{
    public FMODUnity.EventReference sfxEvent;
    public enum NoteType { Quarter, Eighth, Sixteenth, Thirtysecond }
    public NoteType noteType;
    public MusicManager manager;
    private SpriteRenderer sprite;
    private List<Color> colors;
    private int index = 0;
    bool flash = false;
    WaitForSeconds wait = new WaitForSeconds(0.05f);
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        if (noteType == NoteType.Quarter) {
            print("configured to 1/4 notes!");
            manager.handler.beatUpdated += onEvent;
        }
        if (noteType == NoteType.Eighth) {
            print("configured to 1/8 notes!");
            manager.handler.eighthNoteEvent += onEvent;
        }
        if (noteType == NoteType.Sixteenth) {
            print("configured to 1/16 notes!");
            manager.handler.sixteenthNoteEvent += onEvent;
        }
        if (noteType == NoteType.Thirtysecond) {
            print("configured to 1/32 notes!");
            manager.handler.thirtysecondNoteEvent += onEvent;
        }

        colors = new List<Color>();
        colors.Add(Color.red);
        colors.Add(Color.yellow);
        colors.Add(Color.green);
        colors.Add(Color.blue);

        sprite.color = colors[0];
    }

    void Update()
    {
        if (flash == true) {
            CycleColor();
            flash = false;
        }
    }

    void onEvent()
    {
        FMODUnity.RuntimeManager.PlayOneShot(sfxEvent);
        flash = true;
    }

    void CycleColor()
    {
        index = (index+1)%colors.Count;
        sprite.color = colors[index];
    }
}
