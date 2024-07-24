using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonitorPlaytestLog : MonoBehaviour
{
    [SerializeField]
    private StatVarLog log;

    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"id:\n{log.id}\nlog:\n{log.log}";
    }
}
