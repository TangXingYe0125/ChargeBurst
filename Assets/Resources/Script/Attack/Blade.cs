using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private BladeArray _bladeArray;

    private void Start()
    {
        _bladeArray = FindObjectOfType<BladeArray>();
    }  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_bladeArray != null && _bladeArray.isAttackingBoss)
        {
            if (collision.CompareTag("Boss"))
            {
                Destroy(this.gameObject); 
            }
        }
    }
}
