using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unseen_Blade : MonoBehaviour
{
    void Update()
    {
        Destroy(this.gameObject, 0.1f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
    }
}
