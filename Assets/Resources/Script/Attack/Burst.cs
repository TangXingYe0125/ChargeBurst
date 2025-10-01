using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burst : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float _force;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = transform.right * _force;
        Destroy(this.gameObject, 0.5f);
    }
}
