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
    void Update()
    {
        if(HP > 0) 
        { 
            transform.position = Vector3.MoveTowards(transform.position, playerPos.position, speed);

            if (Vector3.Distance(transform.position, playerPos.position) < 1.0f)
            {
                Destroy(this.gameObject);
                SceneManager.LoadScene("Defeat");
            }
        }
        else if(HP <= 0) 
        {
            Destroy(this.gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Burst"))
        {
            HP -= 3;
            Debug.Log("HP:" + HP);
        }
        if (collision.gameObject.CompareTag("Sword"))
        {
            HP -= 1;
            speed -= 0.001f ;
            Debug.Log("HP:" + HP);
        }

    }

}


