using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GoFade _loseScript;
    [SerializeField] private GoFade _winScript;
    void Update()
    {
        if (PlayerHP.instance._HP <= 0)
        {
            _loseScript.StartFade();
        }
        if (InstantiateEnemy.instance._enemyLeft == 0 || Timer.instance._time <= 0.00f)
        {
            _winScript.StartFade();
        }
    }
}
