using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurtainSpriteHandler : MonoBehaviour
{
    [Header("Curtain Data")]
    [SerializeField, Tooltip("The binder of all possible curtain sprites.")]
    UISpriteBinder allCurtains;
    [SerializeField, Tooltip("The text object we write the id of the selected curtain to.")]
    TMP_Text monitorText;
    [SerializeField, Tooltip("The buttons used to increment or decrement our curtain selection.")]
    Button[] curtainChangeButtons;
    /*[SerializeField, Tooltip("The binder of the curtain sprites available by default.")]
    UISpriteBinder baseCurtains;
    [SerializeField, Tooltip("The binder of the curtain sprites currently available.")]
    UISpriteBinder availableCurtains;
    [SerializeField, Tooltip("The curtain sprite for a locked image.")]
    Sprite lockedCurtain;*/


    [SerializeField, Tooltip("The current curtain sprite.")]
    spriteVar curtainSprite;


    private int selection = 0;
    private float lastAspect = -1;
    private bool isSquare;

    private void Awake()
    {
        allCurtains.Initialize();

        for (int i = 0; i < allCurtains.GetCount(); i++) {
            UISpriteData pair = allCurtains.GetPairList()[i];
            if (pair.sprite == curtainSprite.value) {
                selection = i;
                break;
            }
        }

        // If we're here, we didn't get a match.
        // Reset the curtain to match the selection.
        UISpriteData data = allCurtains.GetPairList()[selection];
        curtainSprite.value = data.sprite;
        monitorText.text = data.id;
    }

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

   /* public void UnlockCurtain(string curtainID)
    {
        Sprite curtain = allCurtains.Query(curtainID);
        if (curtain != allCurtains.GetDefault()) {
            // Query was valid! Add the new pair.
            availableCurtains.AddPair(curtainID, curtain);
        } else {
            Debug.LogError($"CurtainSpriteHandler Error: UnlockCurtain failed. " 
                          + "curtainID ({curtainID}) was not found in allCurtains.");
        }
    }

    public void ResetAvailableCurtains()
    {
        availableCurtains.CopyFrom(baseCurtains);
    }*/

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
            print($"Underflowed! Selection is now {selection}");
        }

        UISpriteData data = allCurtains.GetPairList()[selection];
        curtainSprite.value = data.sprite;
        monitorText.text = data.id;
    }
}
