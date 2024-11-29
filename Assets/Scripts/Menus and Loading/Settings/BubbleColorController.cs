using UnityEngine;

public class BubbleColorController : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(70, 10, 50, 50), "Base!"))
        {
            BubbleFlavorMethods.SetColors(BubbleFlavorMethods.GetBaseColors());
        }
        if (GUI.Button(new Rect(70, 70, 50, 50), "HiCon!"))
        {
            BubbleFlavorMethods.SetColors(BubbleFlavorMethods.GetHiConColors());
        }
    }
}