using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class burst : MonoBehaviour
{
    public float force;
    private Rigidbody2D rbody;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.velocity = transform.right * force;
    }
    void Update()
    {
        Destroy(this.gameObject,1.0f);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) 
        {
            Destroy(collision.gameObject);
        }
    }
}
