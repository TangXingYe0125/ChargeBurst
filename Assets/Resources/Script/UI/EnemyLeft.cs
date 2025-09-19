using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLeft : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    void FixedUpdate()
    {
        _text.text = "EnemyLeft:" + InstantiateEnemy.instance._enemyLeft + "/" + InstantiateEnemy.instance._totalAmount;
    }
}
