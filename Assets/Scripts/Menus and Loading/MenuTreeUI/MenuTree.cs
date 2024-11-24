using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTree : MonoBehaviour 
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The text asset describing this menu's tree structure.")]
    private TextAsset treeSource;
    [Tooltip("Something to be displayed if a menu node has no other content.")]
    public BaseMenuContent baseContent;
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
        private set {
            // If overlay is true and content is null, we are prevented from
            // navigating to this MenuTreeNode.
            if (value.terminalOverlay && value.content == null) return;

            // Elsewise, invoke the update action with params (old, new).
            CurrentNodeUpdated?.Invoke(current, value);
            if (!value.terminalOverlay) SetNodeVisible(current, false);
            SetNodeVisible(value, true);
            current = value;
        }
    }
    // A list of IDs being used in our tree already, to avoid doublecounting.
    private HashSet<string> idsInUse = new();
    // Check if we have already called start once before.
    bool calledStart = false;
    // Scenes that count as 'gameplay levels' for the "gameplayOnly" menu tree tag.
    List<string> gameplayScenes = new() { "Level1", "Level2", "Level3", "Level4", "Level5" };

    // ==============================================================
    // Initializers
    // ==============================================================

    private void Awake()
    {
        // Awake is called before Start. We use it to instantiate
        // our menu tree at runtime.
        // ================

        root = ConstructTree();
    }

    private void Start()
    {
        // Start is called before the first frame update.
        // We do our Current = root assignment here, so that other scripts
        // can subscribe to our CurrentNodeUpdated event in Awake.
        // ================
        
        Current = root;
        calledStart = true;
    }

    private void OnEnable()
    {
        // We want the menu to restart when the menu is set to inactive and
        // active again. Thus, if start has already been called before, and
        // we recieve the OnEnable signal, reinitialize.
        // ================

        if (calledStart) {
            Current = null;
            Current = root;
        }
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
                    string[] terms = line.Trim().Split('\\'); // 0th term is the ID, 1st term is the description.
                    rootNode = currentNode = new(terms[0], terms[1], null, GetContent(terms[0]));
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
                    string[] terms = line.Trim().Split('\\'); // 0th term is the ID, 1st term is the description.
                    currentNode = new(terms[0], terms[1], parent, GetContent(terms[0]));

                    // Overrides
                    if (terms.Length > 2) {
                        for (int j = 2; j < terms.Length; j++) {
                            switch (terms[j]) {
                                case "disable":
                                    currentNode.enabled = false;
                                    break;
                                case "overrideTitle":
                                    currentNode.overrideTitle = true;
                                    break;
                                case "gameplayOnly":
                                    string sceneName = SceneManager.GetActiveScene().name;
                                    currentNode.visible = gameplayScenes.Contains(sceneName);
                                    break;
                                case "terminalOverlay":
                                    currentNode.terminalOverlay = true;
                                    break;
                            }
                        }
                    }
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
        if (node != null) { 
            if (node.content != null) {
                node.content.SetActive(visible);
            }

            else { // Node content is null, display base content.
                baseContent.UpdateCurrentNode(node);
                baseContent.ChangeText(node);
                baseContent.gameObject.SetActive(visible);
            }
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
    // The description of the menu
    public string description;
    // Whether or not this node is enabled in the menu
    public bool visible = true;
    // Whether or not this node is enabled in the menu
    public bool enabled = true;
    // Whether or not this node's title is always overridden by it's id
    public bool overrideTitle = false;
    // Whether or not this node's content should be displayed on top of
    // the parent menu's content.
    // If overlay is true and content is null, we are prevented from
    // navigating to this MenuTreeNode.
    public bool terminalOverlay = false;
    // The content of this node.
    public GameObject content = null;
    // The parent to this node.
    public MenuTreeNode parent = null;
    // The children to this node.
    public List<MenuTreeNode> children;

    // ================================
    // Debug values, mainly used in DEBUG_TreeVis.
    public readonly int generation;
    public readonly int siblingIndex;

    public MenuTreeNode(string _id, string _description, MenuTreeNode _parent, GameObject _content=null)
    {
        // Constructor.
        // ================

        id = _id;
        description = _description;
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