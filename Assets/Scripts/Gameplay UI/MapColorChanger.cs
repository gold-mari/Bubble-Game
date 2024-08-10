using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.Mathematics;

[System.Serializable]
public class ColorFrame
{
    [Tooltip("The beatmap name associated with this color changer. ALIGN THIS WITH THE SONG OBJECT.")]
    public string mapName;
    [Tooltip("The base color to be displayed when we go to the named beatmap.")]
    public Color baseColor = Color.black;
    [Tooltip("The orb color to be displayed when we go to the named beatmap.")]
    public Color orbColor = Color.white;
}

public class MapColorChanger : ActionOnSwitchMap
{
    [SerializeField, Tooltip("The list of background color keyframes.")]
    ColorFrame[] backgroundFrames;

    HalftoneBackManager halftoneBackManager;

    private void Awake()
    {
        halftoneBackManager = GetComponent<HalftoneBackManager>();
    }

    protected override void OnSwitchMap(string mapName)
    {
        if (!halftoneBackManager) return;

        foreach (ColorFrame frame in backgroundFrames)
        {
            if (frame.mapName.Contains(mapName))
            {
                halftoneBackManager.baseColor = frame.baseColor;
                halftoneBackManager.orbColor = frame.orbColor;
                return;
            }
        }
    }
}
