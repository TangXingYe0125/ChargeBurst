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
        _text.text = "Kills:" + Kills.instance._kills + "/" + Kills.instance._lastTimeEnemyAmount;
    }
}

