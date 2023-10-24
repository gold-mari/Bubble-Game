using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class ColorFrame
{
    [Tooltip("The beatmap name associated with this color changer. ALIGN THIS WITH THE SONG OBJECT.")]
    public string mapName;
    [Tooltip("The color to be displayed when we go to the named beatmap.")]
    public Color color;
}

public class MapColorChanger : ActionOnSwitchMap
{
    [SerializeField, Tooltip("The list of background color keyframes.")]
    ColorFrame[] backgroundFrames;

    protected override void OnSwitchMap(string mapName)
    {
        foreach (ColorFrame frame in backgroundFrames)
        {
            if (frame.mapName == mapName)
            {
                GetComponent<SpriteRenderer>().color = frame.color;
                return;
            }
        }
    }
}
