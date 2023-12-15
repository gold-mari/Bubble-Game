using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSpriteOnInput : MonoBehaviour
{
    [SerializeField]
    KeyCode key = KeyCode.Mouse0;
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
        if (Input.GetKey(key))
        {
            image.sprite = downSprite;
        }   
        else
        {
            image.sprite = upSprite;
        }
    }
}
