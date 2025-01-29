using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultTime : MonoBehaviour
{
    private TextMeshProUGUI _text;
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        _text.text = "Time:" + (Timer._time).ToString("F2");
    }
}
