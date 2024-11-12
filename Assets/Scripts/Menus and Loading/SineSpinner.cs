using Unity.VisualScripting;
using UnityEngine;

public class SineSpinner : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("Whether we should use unscaled time for our animations.\n\nDefault: false")]
    bool unscaledTime = false;
    [SerializeField, Tooltip("The duration, in seconds, it takes us to complete one cycle.\n\nDefault: 2")]
    float period = 2f;
    [SerializeField, Tooltip("The maximum amount, in degrees around the z-axis, we may rotate.\n\nDefault: 5")]
    float magnitude = 5f;

    // ================================================================
    // Internal variables
    // ================================================================

    static float twopi { get { return 2 * Mathf.PI; } }

    // Update is called once per frame
    void Update()
    {
        float time = unscaledTime ? Time.unscaledTime : Time.time;

        float amt = magnitude * Mathf.Sin(twopi * time / period);
        transform.localRotation = Quaternion.Euler(0,0,amt);
    }
}
