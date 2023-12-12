using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongProgressMeter : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The floatVar representing how far we are into the song.")]
    private floatVar songCompletion;

    // ================================================================
    // Internal variables
    // ================================================================

    private Image image;

    // ================================================================
    // Initializer methods
    // ================================================================

    void Start()
    {
        // Awake is called before Start.
        // ================
        
        image = GetComponent<Image>();  
        image.fillAmount = 0; 
    }

    // ================================================================
    // Update methods
    // ================================================================

    void Update()
    {
        // Update is called once per frame.
        // ================

        image.fillAmount = songCompletion.value;
    }
}
