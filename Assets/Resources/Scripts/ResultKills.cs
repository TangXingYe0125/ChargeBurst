using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultKills : MonoBehaviour
{
    private TextMeshProUGUI _text;
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }
    void FixedUpdate()
    {
        _text.text = "Kills:" + PlayerHP.instance._kills + "/" + InstantiateEnemy.instance._totalAmount;
    }
}
