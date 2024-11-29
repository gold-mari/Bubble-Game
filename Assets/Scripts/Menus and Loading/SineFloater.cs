using Unity.VisualScripting;
using UnityEngine;

public class SineFloater : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("Whether we should use unscaled time for our animations.\n\nDefault: false")]
    bool unscaledTime = false;
    [SerializeField, Tooltip("The duration, in seconds, it takes us to complete one cycle.\n\nDefault: 2")]
    float period = 2f;
    [SerializeField, Tooltip("The maximum amount, in worldspace units, we may float from our base position.\n\nDefault: 1")]
    float magnitude = 1f;

    // ================================================================
    // Internal variables
    // ================================================================

    static float twopi { get { return 2 * Mathf.PI; } }
    private Vector3 baseLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        baseLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float time = unscaledTime ? Time.unscaledTime : Time.time;

        float amt = magnitude * Mathf.Sin(twopi * time / period);
        transform.localPosition = baseLocalPosition + new Vector3(0, amt);
    }
}
