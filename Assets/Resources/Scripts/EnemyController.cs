using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    private Transform playerPos;
    public float speed;
    public int HP = 3;
    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void FixedUpdate()
    {
        if(HP > 0) 
        { 
            transform.position = Vector3.MoveTowards(transform.position, playerPos.position, speed);
        }
        else if(HP <= 0) 
        {
            PlayerHP.instance._kills++;
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Burst"))
        {
            HP -= 3;        
        }
        if (collision.gameObject.CompareTag("Sword"))
        {
            HP -= 1;
            speed -= 0.001f ;
        }
    }
}

