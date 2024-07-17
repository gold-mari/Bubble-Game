using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolVarToFMODParam : MonoBehaviour
{
    [SerializeField, Tooltip("The boolVar applied to our FMOD parameter.")]
    boolVar boolVar;
    [SerializeField, Tooltip("The name FMOD parameter taking our boolVar. Is set to 1 if true, 0 if false.")]
    string fmodParamName;

    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(fmodParamName, boolVar.value ? 1 : 0);
    }
}