using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBlade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            Destroy(this.gameObject);
        }
    }
}
