using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.EventSystems;

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
    [SerializeField, Tooltip("The UISpriteBinder used for our buttons.")]
    private UISpriteBinder iconSprites;
    [SerializeField, Tooltip("The prefab for our buttons.")]
    private GameObject buttonPrefab;

    private ObjectPool buttonPool;
    private ButtonSoundPlayer buttonSoundPlayer;

    private void Awake()
    {
        // Awake is called before Start.
        // ================

        iconSprites.Initialize();

        buttonPool = new ObjectPool(buttonPrefab, dynamicUIParent);
        menuTree.CurrentNodeUpdated += OnCurrentNodeUpdated;

        buttonSoundPlayer = GetComponent<ButtonSoundPlayer>();
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
                    button.interactable = false;
                    button.interactable = true;

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        menuTree.DescendByIndex(index);
                    });
                }
                // Add events for the BaseMenuContent object.
                AddHoverEvents(button, newNode, newNode.children[i]);

                PointsOnCircle.GetArcPosition(anchorLeft.localPosition, anchorCenter.localPosition, anchorRight.localPosition,
                                              i/(float)(childCount-1), Mathf.Min(1, (childCount-1)*0.333f),
                                              out Vector3 position, out Quaternion _);

                buttonObj.transform.localPosition = position;
                
                MenuTreeButton menuTreeButton = buttonObj.GetComponent<MenuTreeButton>();
                menuTreeButton.Initialize(newNode.children[i]);
                menuTreeButton.SetStyle(MenuTreeButton.Style.Main);

                // Also, for debug purposes, name the button.
                buttonObj.name = $"Button ({newNode.children[i].id})";
            }

            // If this isn't the root, create a back button.
            if (newNode != menuTree.root) {
                // Request a new button.
                GameObject buttonObj = buttonPool.Request();
                // Place it.
                PointsOnCircle.GetArcPosition(
                    anchorLeft.localPosition, anchorCenter.localPosition, anchorRight.localPosition,
                    1.25f, 1,
                    out Vector3 position, out var _
                );

                buttonObj.transform.localPosition = position;

                // Initialize the menuTreeButton.
                MenuTreeButton menuTreeButton = buttonObj.GetComponent<MenuTreeButton>();
                menuTreeButton.Initialize(null); // For MenuTreeButtons, null is "Back".
                menuTreeButton.SetStyle(MenuTreeButton.Style.Back);

                // Initialize the button events.
                Button button = buttonObj.GetComponent<Button>();
                if (button) {
                    button.interactable = false;
                    button.interactable = true;
                    
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        menuTree.Ascend();
                    });
                }
                // Add events for the BaseMenuContent object.
                AddHoverEvents(button, newNode, null);

                

                // Also, for debug purposes, name the button.
                buttonObj.name = $"Button (Back)";
            }

            // Ough this is awful, I truly hate this solution.
            // But this deadline is tight, I've got a million other things to do,
            // and I don't know off the top of my head how to distinguish between
            // the SFX event triggers and the hover event triggers.
            buttonSoundPlayer.SupplySFX();
        }
    }

    private void AddHoverEvents(Button button, MenuTreeNode currentNode, MenuTreeNode childNode)
    {
        // Given a button, wipes all of its hover events, and adds a
        // PointerEnter event and a PointerExit event which:
        //  * Spin the button's icon
        //  * Can update the sprite for our menuTree's baseContent
        //
        // AGAIN, IMPORTANT: THIS METHOD WIPES ALL OF THE BUTTON'S TRIGGERS.
        // YOU MUST RE-ADD LOST TRIGGERS ELSEWHERE.
        // ================

        Debug.Log("hover events added for button", button);

        // Find the EventTrigger on this object, or make one if one doesn't exist.
        if (!button.TryGetComponent<EventTrigger>(out var trigger)) {
            trigger = button.gameObject.AddComponent(typeof(EventTrigger)) as EventTrigger;
        } else {
            trigger.triggers.Clear();
        }

        EventTrigger.Entry pointerEnterEvent = new(){ eventID = EventTriggerType.PointerEnter };

        MenuTreeButton treeButton = button.GetComponent<MenuTreeButton>();

        pointerEnterEvent.callback.AddListener((eventData) => { 
            // When we hover over the button, spin our lil icon :3
            treeButton.SpinIcon();
        });
    
        if (currentNode.content == null) {
            // If this node corresponds to a node which uses the base content,
            // change the baseContent's text.

            pointerEnterEvent.callback.AddListener((eventData) => { 
                // When we hover over the button, display the text for the menu we're
                // about to traverse into.
                menuTree.baseContent.ChangeText(childNode);
            });
            EventTrigger.Entry pointerExitEvent = new(){ eventID = EventTriggerType.PointerExit };
            pointerExitEvent.callback.AddListener((eventData) => { 
                // When we stop hovering over a button, display the text for the menu
                // we're already inside of.
                menuTree.baseContent.ChangeText(currentNode);
            });

            // Add our pointerEnterEvent from earlier.
            trigger.triggers.Add(pointerEnterEvent);
            trigger.triggers.Add(pointerExitEvent);
        } 
    }
}