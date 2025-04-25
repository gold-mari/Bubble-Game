using UnityEngine;
using UnityEngine.UI;

public class ChangeSpriteOnInput : MonoBehaviour
{
    public enum Input {
        AFFIRM,
        DENY
    }

    [SerializeField]
    Input input;
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
        if (input == Input.AFFIRM && InputHandler.GetAffirm() || input == Input.DENY && InputHandler.GetDeny()) {
            image.sprite = downSprite;
            return;
        }
        
        image.sprite = upSprite;
    }
}