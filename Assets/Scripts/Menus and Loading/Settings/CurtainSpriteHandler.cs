using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Works in editor!

public class CurtainSpriteHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [Header("Data")]
    [SerializeField, Tooltip("The binder of all possible curtain sprites.")]
    UISpriteBinder allCurtains;
    [SerializeField, Tooltip("The current curtain sprite.")]
    spriteVar curtainSprite;
    [Header("Display")]
    [SerializeField, Tooltip("The text object we write the id of the selected curtain to.")]
    TMP_Text monitorText;
    [SerializeField, Tooltip("The buttons used to increment or decrement our curtain selection.")]
    Button[] curtainChangeButtons;

    // ==============================================================
    // Misc. internal variables
    // ==============================================================

    private int selection = 0;
    private float lastAspect = -1;
    private bool isSquare;

    // ==============================================================
    // Initializers
    // ==============================================================

    public void Initialize()
    {
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, on Awake().
        // ================

        allCurtains.Initialize();

        if (PlayerPrefs.HasKey("CurtainIndexPref")) {
            selection = PlayerPrefs.GetInt("CurtainIndexPref");
        } else { // Default value.
            selection = 0;
        }

        UISpriteData pair = allCurtains.GetPairList()[selection];
        curtainSprite.value = pair.sprite;

        // Force a call to Update, which will update whether or not we can change
        // our current curtain.
        Update();
    }

    private void OnEnable()
    {
        // Make visible and update UI.

        UISpriteData pair = allCurtains.GetPairList()[selection];

        monitorText.text = pair.id;
        curtainSprite.value = pair.sprite;
    }

    private void OnDisable()
    {
        // Save selection to player prefs

        PlayerPrefs.SetInt("CurtainIndexPref", selection);
    }

    // ==============================================================
    // Indexing manipulators
    // ==============================================================

    public void IncrementCurtainSelection()
    {
        if (isSquare) return;

        selection = (selection+1)%allCurtains.GetCount();

        UISpriteData data = allCurtains.GetPairList()[selection];
        curtainSprite.value = data.sprite;
        monitorText.text = data.id;
    }

    public void DecrementCurtainSelection()
    {
        if (isSquare) return;

        selection--;
        // Wrap around if underflow!
        if (selection < 0) {
            selection = allCurtains.GetCount()-1;
        }

        UISpriteData data = allCurtains.GetPairList()[selection];
        curtainSprite.value = data.sprite;
        monitorText.text = data.id;
    }

    // ==============================================================
    // Misc.
    // ==============================================================

    private void Update()
    {
        // Update is called once per frame.
        // We use it to check if the aspect ratio ever changes to square,
        // in which case curtain changing will be disabled.
        // ================

        float aspect = Screen.width/(float)Screen.height;

        if (aspect == lastAspect) return;

        // Otherwise, aspect ratio has changed.
        // If the aspect ratio is square or thinner...
        if (aspect <= 1) {
            // Stop us from changing the curtains. We can't see them, anyway.
            isSquare = true;
            foreach (Button button in curtainChangeButtons) button.interactable = false;
            monitorText.text = "Change Aspect Ratio";
        } else {
            isSquare = false;
            foreach (Button button in curtainChangeButtons) button.interactable = true;
            UISpriteData data = allCurtains.GetPairList()[selection];
            curtainSprite.value = data.sprite;
            monitorText.text = data.id;
        }
        lastAspect = aspect;
    }
}
