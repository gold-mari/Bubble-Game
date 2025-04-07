using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SlideshowDisplay : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The slides to display, in order.")]
    private GameObject[] slides;
    [SerializeField, Tooltip("The button that signals we should advance in the slideshow.")]
    private Button advanceButton;
    [SerializeField, Tooltip("The button that signals we should go back in the slideshow")]
    private Button backButton;
    [SerializeField, ReadOnly, Tooltip("The index of the slide we are currently displaying.")]
    private int currentIndex = 0;
    
    // ==============================================================
    // Internal variables
    // ==============================================================

    // The transform of the current slide we're displaying.
    private GameObject currentSlide;

    // ==============================================================
    // Initialization / finalizations methods
    // ==============================================================

    private void OnEnable()
    {
        // OnEnable is called when this object is enabled. We use it to intialize our slideshow
        // state.
        // ================

        currentIndex = 0;
        advanceButton.interactable = true;
        backButton.interactable = false;

        currentSlide = slides[currentIndex];
        foreach (GameObject slide in slides) {
            slide.SetActive(slide == currentSlide);
        }
    }

    private void OnDisable()
    {
        // OnDisable is called when this object is enabled. We use it to intialize our slideshow
        // state.
        // ================

        currentIndex = 0;
        advanceButton.interactable = true;
        backButton.interactable = false;
    }

    // ==============================================================
    // Slideshow traversal methods
    // ==============================================================

    public void Advance()
    {
        // Called when the advance button is hit.
        // ================

        // Don't do anything if we're at the end of our list.
        if (currentIndex == slides.Length-1) {
            return;
        }

        // Otherwise, advance.
        currentSlide.SetActive(false);

        currentIndex++;
        currentSlide = slides[currentIndex];
        currentSlide.SetActive(true);

        // If we have just advanced, we cannot be at the first slide. Enable the go back button.
        backButton.interactable = true;
        // If we are now at the last slide, disable the advance button.
        if (currentIndex == slides.Length-1)
        {
            advanceButton.interactable = false;
            backButton.Select();
        }
    }

    public void GoBack()
    {
        // Called when the back button is hit.
        // ================

        // Don't do anything if we're at the start of our list.
        if (currentIndex == 0) {
            return;
        }

        // Otherwise, go back.
        currentSlide.SetActive(false);

        currentIndex--;
        currentSlide = slides[currentIndex];
        currentSlide.SetActive(true);

        // If we have just gone back, we cannot be at the last slide. Enable the advance button.
        advanceButton.interactable = true;
        // If we are now at the first slide, disable the go back button.
        if (currentIndex == 0)
        {
            backButton.interactable = false;
            advanceButton.Select();
        }
    }
}
