using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GravityManager : MonoBehaviour
{
    public float gravityStrength = 1;
    public floatVar strengthVar;
    public boolVar gravityFlipped;

    void Start()
    {
        strengthVar.value = -gravityStrength;
        gravityFlipped.value = false;
    }

    public void FlipGravity()
    {
        strengthVar.value *= -1;
        gravityFlipped.value = !gravityFlipped.value;
    }
}
