using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class InitializeSettings : MonoBehaviour
{
    private void Awake()
    {
        // Initializes the settings handler under us, and then disables us.
        // ================

        SettingsHandler handler = GetComponentInChildren<SettingsHandler>(includeInactive:true);
        if (handler != null) {
            handler.Initialize();
        }

        gameObject.SetActive(false);
    }
}
