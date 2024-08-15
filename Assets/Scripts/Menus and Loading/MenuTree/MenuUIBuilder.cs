using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class MenuUIBuilder : MonoBehaviour 
{
    [Header("Scene References")]
    [SerializeField, Tooltip("The MenuTree that we're building UI from.")]
    private MenuTree menuTree;
    [SerializeField, Tooltip("The transform under which we will maintain all dynamic menu UI.")]
    private Transform dynamicUIParent;
    [SerializeField, Tooltip("The transform representing the left anchor of our button arc.")]
    private Transform anchorLeft;
    [SerializeField, Tooltip("The transform representing the center anchor of our button arc.")]
    private Transform anchorCenter;
    [SerializeField, Tooltip("The transform representing the right anchor of our button arc.")]
    private Transform anchorRight;
    [Header("Prefab References")]
    [SerializeField, Tooltip("The prefab for our buttons.")]
    private GameObject buttonPrefab;

    private ObjectPool buttonPool;

    private void Awake()
    {
        // Awake is called before Start.
        // ================

        buttonPool = new ObjectPool(buttonPrefab, dynamicUIParent, 3);
        menuTree.CurrentNodeUpdated += OnCurrentNodeUpdated;
    }

    private void OnCurrentNodeUpdated(MenuTreeNode oldNode, MenuTreeNode newNode) 
    {   
        // Called whenever the current node in our menuTree changes. Used to:
        //  * Update our visible buttons, using our buttonPool
        //  * Do other things (ADD THEM HERE)
        // ================

        buttonPool.DeactivateAll();

        if (newNode != null) {
            // For each child in our menu...
            int childCount = newNode.children.Count;
            for (int i=0; i<childCount; i++)
            {
                // We need to redeclare this variable, otherwise the onClick event
                // delegate stores the reference to i, instead of the value.
                int index = i;
                // Request a new button and set up the listener.
                GameObject buttonObj = buttonPool.Request();
                Button button = buttonObj.GetComponent<Button>();
                if (button) {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        menuTree.DescendByIndex(index);
                    });
                }

                PointsOnCircle.GetArcPosition(anchorLeft.localPosition, anchorCenter.localPosition, anchorRight.localPosition,
                                              i/(float)(childCount-1), Mathf.Min(1, (childCount-1)*0.333f),
                                              out Vector3 position, out Quaternion _);

                buttonObj.transform.localPosition = position;
                
                MenuTreeButton menuTreeButton = buttonObj.GetComponent<MenuTreeButton>();
                menuTreeButton.ChangeText(newNode.children[i].id);
                menuTreeButton.SetStyle(MenuTreeButton.Style.Main);
            }

            // If this isn't the root, create a back button.
            if (newNode != menuTree.root) {
                // Request a new button and set up the listener.
                GameObject buttonObj = buttonPool.Request();
                Button button = buttonObj.GetComponent<Button>();
                if (button) {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        menuTree.Ascend();
                    });
                }

                PointsOnCircle.GetArcPosition(anchorLeft.localPosition, anchorCenter.localPosition, anchorRight.localPosition,
                                              1.25f, 1,
                                              out Vector3 position, out Quaternion _);

                buttonObj.transform.localPosition = position;
                print($"Spawned Back at {position}");

                MenuTreeButton menuTreeButton = buttonObj.GetComponent<MenuTreeButton>();
                menuTreeButton.ChangeText("Back");
                menuTreeButton.SetStyle(MenuTreeButton.Style.Back);
            }
        }
    }
}