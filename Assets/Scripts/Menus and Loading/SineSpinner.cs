using Unity.VisualScripting;
using UnityEngine;

public class SineSpinner : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The duration, in seconds, it takes us to complete one cycle.")]
    float period = 2f;
    [SerializeField, Tooltip("The maximum amount, in degrees around the z-axis, we may rotate.")]
    float magnitude = 5f;

    // ================================================================
    // Internal variables
    // ================================================================

    static float twopi
    { 
        get { return 2 * Mathf.PI; }
    }

    // Update is called once per frame
    void Update()
    {
        float amt = magnitude * Mathf.Sin(twopi * Time.time / period);
        transform.rotation = Quaternion.Euler(0,0,amt);
    }
}
