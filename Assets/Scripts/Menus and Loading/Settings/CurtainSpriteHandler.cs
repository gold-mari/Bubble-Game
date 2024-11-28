using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurtainSpriteHandler : MonoBehaviour
{
    [Header("Curtain Data")]
    [SerializeField, Tooltip("The binder of all possible curtain sprites.")]
    UISpriteBinder allCurtains;
    [SerializeField, Tooltip("The text object we write the id of the selected curtain to.")]
    TMP_Text monitorText;
    /*[SerializeField, Tooltip("The binder of the curtain sprites available by default.")]
    UISpriteBinder baseCurtains;
    [SerializeField, Tooltip("The binder of the curtain sprites currently available.")]
    UISpriteBinder availableCurtains;
    [SerializeField, Tooltip("The curtain sprite for a locked image.")]
    Sprite lockedCurtain;*/


    [SerializeField, Tooltip("The current curtain sprite.")]
    spriteVar curtainSprite;


    private int selection = 0;

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
        selection = (selection+1)%allCurtains.GetCount();

        UISpriteData data = allCurtains.GetPairList()[selection];
        curtainSprite.value = data.sprite;
        monitorText.text = data.id;
    }

    public void DecrementCurtainSelection()
    {
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
