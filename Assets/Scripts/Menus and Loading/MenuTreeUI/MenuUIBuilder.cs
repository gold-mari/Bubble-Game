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
    [SerializeField, Tooltip("The MenuArcZoom component in our scene.")]
    private MenuArcZoom menuArcZoom;
    [Header("Prefab References")]
    [SerializeField, Tooltip("The UISpriteBinder used for our buttons.")]
    private UISpriteBinder iconSprites;
    [SerializeField, Tooltip("The prefab for our buttons.")]
    private GameObject buttonPrefab;
    [Header("Parameters")]
    [SerializeField, Tooltip("Whether we should use unscaled time for our animations.\n\nDefault: false")]
    bool unscaledTime = false;
    [SerializeField, Tooltip("The amount of time, in seconds, it takes for the menu arc to zoom in/out."
                            +"\n\nDefault: 0.2")]
    private float menuArcZoomTime = 0.2f;

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
        // ================

        buttonPool.DeactivateAll();

        if (newNode != null) {
            // Get a list of visible children.
            List<MenuTreeNode> visibleChildren = new();
            foreach (MenuTreeNode child in newNode.children) {
                if (child.visible) visibleChildren.Add(child);
            }

            // For each child in our menu...
            int childCount = visibleChildren.Count;
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

                    button.interactable = false;
                    if (visibleChildren[i].enabled) {
                        button.interactable = true;
                    }

                    // Add events for the BaseMenuContent object.
                    AddHoverEvents(button, newNode, visibleChildren[i]);
                }

                float progress = (childCount > 1) ? i/(float)(childCount-1) : 0.75f;
                float usableRange = Mathf.Min(1, (childCount-1)*0.333f);

                PointsOnCircle.GetArcPosition(anchorLeft.localPosition, anchorCenter.localPosition, anchorRight.localPosition,
                                              progress, usableRange, out Vector3 position, out Quaternion _);

                buttonObj.transform.localPosition = position;
                
                MenuTreeButton menuTreeButton = buttonObj.GetComponent<MenuTreeButton>();
                menuTreeButton.Initialize(visibleChildren[i], i, unscaledTime);
                menuTreeButton.SetStyle(MenuTreeButton.Style.Main);

                // Also, for debug purposes, name the button.
                buttonObj.name = $"Button ({visibleChildren[i].id})";
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
                // For MenuTreeButtons, null is "Back".
                menuTreeButton.Initialize(null, -1, unscaledTime);
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

            // Zoom in/out depending on whether or not the new node has content.
            StopAllCoroutines();
            bool skipAnimation = oldNode == null; // If the old node was null, skip the animation.
            StartCoroutine(ZoomRoutine(newNode, skipAnimation));
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

    private IEnumerator ZoomRoutine(MenuTreeNode node, bool impatient=false)
    {
        float start = menuArcZoom.ZoomAmount;
        float end = (node.content == null || node.alsoShowBase) ? 0 : 1;

        if (!impatient) {
            float distance = Mathf.Abs(end-start);
            float duration = menuArcZoomTime*distance; // If we have less to go, take less time.

            float elapsed = 0;
            while (elapsed < duration) {
                menuArcZoom.ZoomAmount = Mathf.Lerp(start, end, LerpKit.EaseInOut(elapsed/duration));
                float deltaTime = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                elapsed += deltaTime;
                yield return null;
            }
        }

        menuArcZoom.ZoomAmount = end;
    }
}