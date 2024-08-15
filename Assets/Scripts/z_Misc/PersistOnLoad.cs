using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistOnLoad : MonoBehaviour
{
    // ==============================================================
    // Default methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to call 
        // DontDestroyOnLoad on this object.
        // ================

        DontDestroyOnLoad(gameObject);
    }
}
