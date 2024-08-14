using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTree : MonoBehaviour 
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The text asset describing this menu's tree structure.")]
    private TextAsset treeSource;
    [SerializeField, Tooltip("A list of string-GameObject, ID-Content pairs.")]
    private List<ContentIDPair> contentIDPairs;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // An action called whenever the current node changes.
    // The first MenuTreeNode arg is the old node, the second is the new node.
    public System.Action<MenuTreeNode, MenuTreeNode> CurrentNodeUpdated;
    [HideInInspector] // The root node of our tree.
    public MenuTreeNode root = null;
    
    // The current node of our tree.
    private MenuTreeNode current = null;
    public MenuTreeNode Current {
        get { return current; }
        set {
            // Invoke the update action with params (old, new).
            CurrentNodeUpdated?.Invoke(current, value);
            SetNodeVisible(current, false);
            SetNodeVisible(value, true);
            current = value;
        }
    }
    // A list of IDs being used in our tree already, to avoid doublecounting.
    private HashSet<string> idsInUse = new();

    // ==============================================================
    // Initializers
    // ==============================================================

    private void Awake()
    {
        // Awake is called before Start. We use it to instantiate
        // our menu tree at runtime.
        // ================

        root = ConstructTree();
        print("=============");
        print(root.id);
    }

    private void Start()
    {
        // Start is called before the first frame update.
        // We do our Current = root assignment here, so that other scripts
        // can subscribe to our CurrentNodeUpdated event in Awake.
        // ================
        
        Current = root;
    }

    private MenuTreeNode ConstructTree()
    {
        // Constructs a tree from our menuTree text asset.
        // Returns the root node.
        // ================

        string[] lines = treeSource.text.Split('\n');
        MenuTreeNode rootNode = null;

        int lastIndent = 0;
        MenuTreeNode lastNode = null;
        MenuTreeNode currentNode = null;

        for (int i = 0; i < lines.Length; i++) {
            string line = lines[i];
            
            // ================================
            // Part 1: Calculate indent level
            // ================================
            int indentLevel = 0;
            foreach (char c in line) {
                if (char.IsWhiteSpace(c)) { indentLevel++; }
                else { break; }
            }

            // ================================
            // Part 2: Check to make root
            // ================================
            if (indentLevel == 0) {
                // Should be the root. Check if we have one already.
                if (rootNode != null) {
                    Debug.LogError("MenuTree Error. ConstructTree failed. " +
                                   "More than one root node was found in treeSource.", this);
                    return null;
                } else {
                    string currentID = line.Trim();
                    rootNode = currentNode = new(currentID, null, GetContent(currentID));
                }
            }

            // ================================
            // Part 3: Check to make child nodes
            // ================================
            else {
                MenuTreeNode parent;

                if (indentLevel > lastIndent) {
                    // Greater indent, child node.
                    parent = lastNode;
                } else if (indentLevel == lastIndent) {
                    // Same indent, sibling node.
                    parent = lastNode.parent;
                } else { // if (indentLevel < lastIndent)
                    // Lesser indent, aunt node.
                    parent = lastNode.parent.parent;
                }

                if (idsInUse.Contains(line.Trim())) {
                    Debug.LogError($"MenuTree Error. ConstructTree failed. " +
                              $"More than one node named '{line.Trim()}' was found in treeSource.", this);
                    return null;
                } else {
                    string currentID = line.Trim();
                    currentNode = new(currentID, parent, GetContent(currentID));
                }
            }

            // Hide!
            SetNodeVisible(currentNode, false);

            // ================================
            // Part 4: Continue looping
            // ================================
            lastIndent = indentLevel;
            lastNode = currentNode;
            idsInUse.Add(line.Trim());
        }
        
        return rootNode;

        GameObject GetContent(string currentID) 
        {
            // Local method for getting the content from an ID, from
            // our contentIDPairs list.
            // ================
            List<ContentIDPair> pairs = contentIDPairs.FindAll(pair => pair.id == currentID);

            if (pairs.Count > 1) {
                // Error. Multiple instances of one ID.
                Debug.LogError("MenuTree Error: ConstructTree.GetContent failed. " +
                              $"Multiple instances of ID {currentID} were found in the ContentIDPair list.", this);
                return null;
            } else if (pairs.Count == 1) {
                return pairs[0].content;
            } else { // pairs.Count == 0
                return null;
            }
        }
    }

    // ==============================================================
    // Manipulator methods
    // ==============================================================

    public void DescendByName(string id) 
    {
        // Go down a level in our tree, based on the child names.
        // ================

        MenuTreeNode target = Current.children.Find(child => child.id == id);

        if (target == null) {
            Debug.LogError($"MenuTree Error: DescendByName failed. Name '{id}' not found.", this);
            return;
        }

        Current = target;
    }

    public void DescendByIndex(int index) 
    {
        // Go down a level in our tree, based on the sibling indices.
        // ================

        if (index < 0 || index >= Current.children.Count) {
            Debug.LogError($"MenuTree Error: DescendByIndex failed. Index ({index}) out of bounds.", this);
            return;
        }

        Current = Current.children[index];
    }

    public void Ascend() 
    {
        // Go up a level in our tree, unless we're at the root.
        // If we're at the root, do some minor feedback thing.
        // ================

        if (Current != root) {
            Current = Current.parent;
        } else {
            Debug.Log("MenuTree Message: Attempted to call Ascend from root.", this);
        }
    }

    // ==============================================================
    // Misc methods
    // ==============================================================

    private void SetNodeVisible(MenuTreeNode node, bool visible)
    {
        if (node != null && node.content != null) {
            node.content.SetActive(visible);
        }
    }
}

// ==============================================================
// Helper classes
// ==============================================================

public class MenuTreeNode
{
    // The display name of the menu
    public string id;
    // The to this node.
    public GameObject content = null;
    // The parent to this node.
    public MenuTreeNode parent = null;
    // The children to this node.
    public List<MenuTreeNode> children;

    // ================================
    // Debug values, mainly used in DEBUG_TreeVis.
    public readonly int generation;
    public readonly int siblingIndex;

    public MenuTreeNode(string _id, MenuTreeNode _parent, GameObject _content=null)
    {
        // Constructor.
        // ================

        id = _id;
        parent = _parent;
        content = _content;
        children = new List<MenuTreeNode>();

        if (parent != null) {
            parent.children.Add(this);
            generation = parent.generation + 1;
            siblingIndex = parent.siblingIndex + parent.children.Count - 1;
        } else {
            generation = 0;
            siblingIndex = 0;
        }
    }
}

[System.Serializable]
public struct ContentIDPair
{
    public string id;
    public GameObject content;
}