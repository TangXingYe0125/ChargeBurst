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
    private Rigidbody2D _rb;
    [SerializeField]private float _force;
    [SerializeField]private SpriteRenderer _sr;
    private float _time;
    private Color NewColor;
    private void Start()
    {
        NewColor = _sr.color;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        _sr.color = NewColor;
        if (_sr.color.a == 0.5f)
        {
            _time += Time.deltaTime;
            if (_time >= 0.4f)
            {
                NewColor.a = 1.0f;
                _time = 0.0f;
            }
        }
        if (HP > 0) 
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
            FeedBack();
            speed -= 0.001f ;     
        }
    }
    private void FeedBack()
    {  
        NewColor.a = 0.5f;     
        if (this.transform.position.x >= playerPos.position.x)
        {
            _rb.AddForce(new Vector2(_force, 0.0f), ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector2(-_force, 0.0f), ForceMode2D.Impulse);
        }
        if (this.transform.position.y >= playerPos.position.y)
        {
            _rb.AddForce(new Vector2(0.0f, _force), ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector2(0.0f, -_force), ForceMode2D.Impulse);
        }
    }
}

