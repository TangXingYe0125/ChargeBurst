using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLeft : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private InstantiateEnemy _instantiateEnemy;
    void Update()
    {
        _text.text = "EnemyLeft:" + _instantiateEnemy._enemyLeft + "/" + _instantiateEnemy._totalAmount;
    }
}
