using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    private Transform _playerPos;
    public float _speed;
    public float _speedDownRate;
    public int _hp = 3;
    private Rigidbody2D _rb;
    [SerializeField]private float _force;
    [SerializeField]private SpriteRenderer _sr;
    private float _time;
    private float _damageTime = 0.4f;
    private Color NewColor;
    private void Start()
    {
        NewColor = _sr.color;
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        _sr.color = NewColor;
        if (_sr.color.a == 0.5f)
        {
            _time += Time.deltaTime;
            if (_time >= _damageTime)
            {
                NewColor.a = 1.0f;
                _time = 0.0f;
            }
        }
        if (_hp > 0) 
        { 
            transform.position = Vector3.MoveTowards(transform.position, _playerPos.position, _speed);
        }
        else if(_hp <= 0) 
        {
            PlayerHP.instance._kills++;
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Burst"))
        {
            _hp -= 3;            
        }
        if (collision.gameObject.CompareTag("Sword"))
        {
            _hp -= 1;
            FeedBack();
            _speed -= 1.00f * _speedDownRate ;     
        }
    }
    private void FeedBack()
    {  
        NewColor.a = 0.5f;     
        if (this.transform.position.x >= _playerPos.position.x)
        {
            _rb.AddForce(new Vector2(_force, 0.0f), ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector2(-_force, 0.0f), ForceMode2D.Impulse);
        }
        if (this.transform.position.y >= _playerPos.position.y)
        {
            _rb.AddForce(new Vector2(0.0f, _force), ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(new Vector2(0.0f, -_force), ForceMode2D.Impulse);
        }
    }
}

