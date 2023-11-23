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

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();   
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = songCompletion.value;
    }
}
