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
    private Dictionary<string, Sprite> intDict = new();

    public void Initialize()
    {
        // Initializes our dict from our list, called from the UI builder.
        // ================

        pairDict.Clear();
        foreach (UISpriteData pair in pairList) {
            pairDict[pair.id] = pair.sprite;
        }
    }

    public void CopyFrom(UISpriteBinder other)
    {
        // Copy over the pair list.
        pairList.Clear();
        pairList.AddRange(other.pairList);

        // Reinitialize the pair dict.
        Initialize();
    }

    public void AddPair(string id, Sprite sprite)
    {
        if (pairDict.ContainsKey(id)) {
            Debug.LogError($"UISpriteBinder Warning: Called AddPair on an id ({id}) that already existed in pairDict.");
            return;
        }

        pairDict[id] = sprite;
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

    public List<UISpriteData> GetPairList()
    {
        return pairList;
    }

    public int GetCount()
    {
        return pairDict.Keys.Count;
    }
}

[System.Serializable]
public class UISpriteData
{
    public string id;
    public Sprite sprite;

    public UISpriteData(string _id, Sprite _sprite)
    {
        id = _id;
        sprite = _sprite;
    }
}