using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private BladeArray _bladeArray;

    private void Start()
    {
        // 找到场景里的 BladeArray
        _bladeArray = FindObjectOfType<BladeArray>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 只有在发动总攻击时，飞剑才会消失
        if (_bladeArray != null && _bladeArray.isAttackingBoss)
        {
            if (collision.CompareTag("Boss"))
            {
                Destroy(this.gameObject);  // 飞剑消失
            }
        }
    }
}
