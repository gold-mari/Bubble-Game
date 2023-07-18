using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bubble_Color {NONE, Red, Blue, Yellow, Green}

public class Bubble_Color_Methods : MonoBehaviour
{
    public static int length = System.Enum.GetValues(typeof(Bubble_Color)).Length;

    //================================================================================================================

    // Returns a random regular type.
    public static Bubble_Color random()
    {
        return (Bubble_Color)Random.Range(1, length);
    }
}
