using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DEBUG_TreeVis : MonoBehaviour 
{
    public MenuTree menuTree;

    public Canvas canvas;
    public GameObject visNodePrefab;
    public GameObject buttonPrefab;
    public List<Button> buttons;


    private Vector2 dimensions;
    public Dictionary<MenuTreeNode, GameObject> nodeToVisNode = new();

    private void Awake()
    {
        menuTree.CurrentNodeUpdated += (MenuTreeNode oldNode, MenuTreeNode newNode) => {
            string o = (oldNode == null) ? "nothing" : oldNode.id;
            string n = (newNode == null) ? "nothing" : newNode.id;
            print($"Node changed from {o} to {n}");
        };
        menuTree.CurrentNodeUpdated += OnCurrentNodeUpdated;
    }

    private void Start()
    {
        if (menuTree.root != null) {
            // Execute a DFS traversal from the root.
            HashSet<MenuTreeNode> visited = new();
            Stack<MenuTreeNode> searchStack = new();
            searchStack.Push(menuTree.root);

            Rect visNodeRect = visNodePrefab.GetComponent<RectTransform>().rect;
            dimensions = new(visNodeRect.width, visNodeRect.height);

            for (int i=0; searchStack.Count>0; i++) {
                // Pop off the stack!
                MenuTreeNode current = searchStack.Pop();
                visited.Add(current);

                // Instantiate...
                GameObject visNode = Instantiate(visNodePrefab, canvas.transform);
                visNode.name = $"VisNode ({current.id})";
                visNode.GetComponentInChildren<TMP_Text>().text = current.id;
                nodeToVisNode[current] = visNode;
                // And place.
                visNode.transform.position = visNodePrefab.transform.position + new Vector3(
                    current.siblingIndex*1.2f*dimensions.x,
                    -current.generation*1.2f*dimensions.y,
                    0
                );

                // Continue traversing!
                if (current != null) {
                    foreach (MenuTreeNode child in current.children) {
                        if (!visited.Contains(child) && child != null) {
                            searchStack.Push(child);
                        }
                    }
                }
            }
        }
        
        visNodePrefab.SetActive(false);
        buttonPrefab.SetActive(false);
    }

    private void Update()
    {
        foreach (MenuTreeNode node in nodeToVisNode.Keys) {
            GameObject visNode = nodeToVisNode[node];
            
            // Draw lines from child to parent.
            if (node.parent != null) {
                Debug.DrawLine(
                    visNode.transform.position, 
                    nodeToVisNode[node.parent].transform.position,
                    Color.green
                );
            }

            // Draw lines from parent to child.
            foreach (MenuTreeNode child in node.children) {
                Debug.DrawLine(
                    visNode.transform.position, 
                    nodeToVisNode[child].transform.position + Vector3.one*3f,
                    Color.red
                );
            }
        }
    }

    private void OnCurrentNodeUpdated(MenuTreeNode oldNode, MenuTreeNode newNode) 
    {
        if (oldNode != null) {
            // Dehighlight the old node.
            nodeToVisNode[oldNode].GetComponent<Image>().color = Color.white;
        }
        
        if (newNode != null) {
            // Highlight the new node.
            nodeToVisNode[newNode].GetComponent<Image>().color = Color.yellow;

            // Make the navigation buttons!
            foreach (Button button in buttons) {
                Destroy(button.gameObject);
            }
            buttons.Clear();

            // For each child, and then once more...
            for (int i=0; i<newNode.children.Count; i++)
            {
                int index = i;
                // Create a new button in the right place...
                Button buttonObj = CreateButton(newNode, index);
                buttonObj.onClick.AddListener(() => {

                    // ================================
                    // TODO:
                    // Need to find a way to capture the value of this i rather than the ref!
                    // ================================

                    menuTree.DescendByIndex(index); //(newNode.children[i].id);
                });

                buttons.Add(buttonObj);
            }

            // If this isn't the root, create a back button.
            if (newNode != menuTree.root) {
                // Create a new button in the right place...
                Button backButtonObj = CreateButton(newNode, newNode.children.Count+1);
                backButtonObj.onClick.AddListener(() => {
                    menuTree.Ascend();
                });

                buttons.Add(backButtonObj);
            }
        }

        Button CreateButton(MenuTreeNode current, int indexInMenu)
        {
            // Creates and returns a new button object.
            // ================

            Button buttonObj = Instantiate(buttonPrefab, canvas.transform).GetComponentInChildren<Button>();
            buttonObj.transform.position = new(Screen.width / 2f, Screen.height / 2f - Screen.height * 0.05f * indexInMenu);
            string buttonText = (indexInMenu < current.children.Count) ? current.children[indexInMenu].id : "Back";
            buttonObj.GetComponentInChildren<TMP_Text>().text = buttonObj.name = buttonText;

            buttonObj.gameObject.SetActive(true);
            return buttonObj;
        }
    }
}