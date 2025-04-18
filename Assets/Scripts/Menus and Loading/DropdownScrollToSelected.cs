using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    Adapted from ScrollRectAutoScroll.cs by mandarinx.
    Accessed from https://gist.github.com/mandarinx/eae10c9e8d1a5534b7b19b74aeb2a665
*/

[RequireComponent(typeof(ScrollRect))]
public class DropdownScrollToSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Parameters and publics =====================================================================

    [SerializeField, Tooltip("The speed, in pixels per second, of how quickly we autoscroll.")]
    private float scrollSpeed = 10f;

    // Misc internal variables ====================================================================

    private GameObject _lastSelectedObject = null;
    private Selectable _lastSelected = null;
    private bool _mouseOver = false;
    private List<Selectable> _selectables = new List<Selectable>();
    private ScrollRect _scrollRect;
    private Vector2 _nextScrollPosition = Vector2.up;

    // Initializers ===============================================================================

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        // Obtain our selectables.
        if (_scrollRect) {
            _scrollRect.content.GetComponentsInChildren(_selectables);
        }

        // Initialize our dirty flags.
        _lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        _lastSelected = _lastSelectedObject ? _lastSelectedObject.GetComponent<Selectable>() : null;

        ScrollToSelected(_lastSelected, true);
    }

    private void OnEnable()
    {
        // Every time this rect re-enables, re-obtain our selectables.
        if (_scrollRect) {
            _scrollRect.content.GetComponentsInChildren(_selectables);
        }
    }

    // Update methods =============================================================================

    private void Update()
    {
        if (_selectables.Count > 0) {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            // If our selected object has changed, update our selectable.
            if (_lastSelectedObject != selectedObject) {
                _lastSelectedObject = selectedObject;
                _lastSelected = selectedObject ? selectedObject.GetComponent<Selectable>() : null;
            }

            ScrollToSelected(_lastSelected, false);
        }

        if (!_mouseOver) {
            // Lerp scrolling code.
            _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, _nextScrollPosition, scrollSpeed*Time.unscaledDeltaTime);
        } else {
            _nextScrollPosition = _scrollRect.normalizedPosition;
        }
    }

    private void ScrollToSelected(Selectable selected, bool quickScroll)
    {
        int selectedIndex = _selectables.IndexOf(selected);

        if (selectedIndex > -1) {
            Vector2 pos = new(0, 1-(selectedIndex/((float)_selectables.Count-1)));

            if (quickScroll) {
                // Force an immediate update in our Lerp function in Update().
                _scrollRect.normalizedPosition = pos;
                _nextScrollPosition = _scrollRect.normalizedPosition;
            } else {
                _nextScrollPosition = pos;
            }
        }    
    }

    // Pointer event handlers =====================================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseOver = false;
        ScrollToSelected(_lastSelected, false);
    }
}