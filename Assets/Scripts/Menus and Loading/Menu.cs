using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField, Tooltip("The child of this object to display on enabling.")]
    GameObject defaultChild;

    private void Awake()
    {
        // Run before Start. Makes sure defaultChild is a child of us.
        // ================

        Debug.Assert(defaultChild.transform.IsChildOf(transform), "Menu Error: Awake() failed. defaultChild is not a child of the " +
                                                                   "Menu GameObject.", this);
    }

    private void OnEnable()
    {
        // Run on enabling. Enable the designated child, and disable all others.
        // ================

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child == defaultChild)
            {
                child.SetActive(true);
            }
            else
            {
                child.SetActive(false);
            }
        }
    }
}
