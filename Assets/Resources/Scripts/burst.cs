using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class burst : MonoBehaviour
{
    [SerializeField] private float force;
    private Rigidbody2D rbody;
    [SerializeField] private float _speed;

    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.velocity = transform.right * force;
    }
    void Update()
    {
        this.transform.localScale *= 1 + Time.deltaTime * _speed;
        Destroy(this.gameObject,1.0f);
    }
}
