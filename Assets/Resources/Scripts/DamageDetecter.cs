using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetecter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && PlayerHP.instance._isReady == true)
        {
            PlayerHP.instance._isEnemy = true;
            Destroy(collision.gameObject);
        }
    }
}
