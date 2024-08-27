using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UI Sprite Binder", menuName = "Sprite Binders/UI Sprite Binder")]
public class UISpriteBinder : ScriptableObject
{
    [SerializeField, Tooltip("The sprite displayed when a null object is queried.")]
    private Sprite defaultSprite;
    [SerializeField, Tooltip("A list of string-sprite pairs that are converted at runtime to a queryable dict.")]
    private List<UISpriteData> pairList;
    // The queryable dict.
    private Dictionary<string, Sprite> pairDict = new();

    public void Initialize()
    {
        // Initializes our dict from our list, called from the UI builder.
        // ================

        pairDict.Clear();
        foreach (UISpriteData pair in pairList) {
            pairDict[pair.id] = pair.sprite;
        }
    }

    public Sprite Query(string queriedID)
    {
        // Attempts to find queriedID in pairDict, and return the associated sprite.
        // Returns defaultSprite if it couldn't find it.
        // ================

        if (pairDict.ContainsKey(queriedID)) {
            return pairDict[queriedID];
        } else {
            return defaultSprite;
        }
    }

    public Sprite GetDefault()
    {
        return defaultSprite;
    }
}

[System.Serializable]
public class UISpriteData
{
    public string id;
    public Sprite sprite;
}