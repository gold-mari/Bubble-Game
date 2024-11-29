using UnityEngine;

public class BubbleColorController : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(70, 10, 50, 50), "Base!"))
        {
            PresetBase();
        }
        if (GUI.Button(new Rect(70, 70, 50, 50), "HiCon!"))
        {
            PresetHighContrast();
        }
    }

    public void PresetBase()
    {
        BubbleFlavorMethods.SetColors(BubbleFlavorMethods.GetBaseColors());
    }

    public void PresetHighContrast()
    {
        BubbleFlavorMethods.SetColors(BubbleFlavorMethods.GetHiConColors());
    }


}