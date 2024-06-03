using UnityEngine;

// This file contains functions used to extend lerping to include different eases.
// Code adapted from: https://www.febucci.com/2018/08/easing-functions/.
public class LerpKit
{
    // ==============================================================
    // Static methods
    // ==============================================================

    public static float Flip(float t)
    {
        // Returns the difference between 1 and t. Because lerping operates from 0-1,
        // this effectively inverts a lerp amount.
        // ================

        return 1-t;
    }

    public static float EaseIn(float t, float power=2)
    {
        // EaseIn starts slow and speeds up, like an exponential curve. power controls
        // how dramatically slope ramps up.
        // ================

        return Mathf.Pow(t,power);
    }

    public static float EaseOut(float t, float power=2)
    {
        // EaseIn starts fast and slows down, like a logarithmic curve. power controls
        // how dramatically slope ramps down.
        // ================

        return Flip(EaseIn(Flip(t), power));
    }

    public static float EaseInOut(float t, float power=2)
    {
        // EaseIn starts slow, speeds up, and slows down, like a cubic curve. power
        // controls how dramatically slope changes.

        // Instead of lerping between EaseIn() and EaseOut(), switches between
        // them using a ternary. Lerping flattens the curve at high power.

        // LERP CURVE (Do Not Use)
        // return Mathf.Lerp(EaseIn(t,power), EaseOut(t,power), t);
        // ================

        // At t == 0.5, these functions both evaluate to 0.5.
        return (t <= 0.5) ? EaseIn(2*t, power)/2
                          : (EaseOut(2*t - 1, power) + 1)/2;
    }
}
