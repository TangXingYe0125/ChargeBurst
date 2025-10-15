using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kills : MonoBehaviour
{
    public static Kills instance;
    public int _kills = 0;
    public int _explodes = 0;
    public int _lastTimeEnemyAmount;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    public void GetAmount()
    {
        _lastTimeEnemyAmount = GameObject.Find("EnemyController").GetComponent<InstantiateEnemy>()._totalAmount;
    }

    public void ResetKills()
    {
        _kills = 0;
        _explodes = 0;
        _lastTimeEnemyAmount = 0;
    }
}
