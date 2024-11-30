using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpriteOnInput : MonoBehaviour
{
    [SerializeField]
    KeyCode[] keys;
    [SerializeField]
    Sprite upSprite, downSprite;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode key in keys) {
            if (Input.GetKey(key)) {
                image.sprite = downSprite;
                return;
            }
        }
        
        image.sprite = upSprite;
    }
}
