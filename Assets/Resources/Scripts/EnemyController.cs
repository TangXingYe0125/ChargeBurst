using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    private Transform _playerPos;
    [SerializeField] private float _speed;
    public int _hp = 3;
    private Rigidbody2D _rb;
    [SerializeField]private float _force;
    [SerializeField]private SpriteRenderer _sr;
    [SerializeField]private float _damageTime;
    private Color NewColor;
    private Vector2 _knockbackVelocity;
    private bool _isFeedingBack;
    private void Start()
    {
        NewColor = _sr.color;
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _isFeedingBack = false;
    }

    private void FixedUpdate()
    {
        if (_hp > 0 && !_isFeedingBack)
        {
            Vector2 direction = (_playerPos.position - transform.position).normalized;
            _rb.velocity = direction * _speed;
        }
        else if(_hp <= 0)
        {
            _rb.velocity = Vector2.zero;
            PlayerHP.instance._kills++;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Burst"))
        {
            _hp -= 3;
        }
        else if (collision.CompareTag("Sword"))
        {
            _hp -= 1;
            StartCoroutine(FeedBack());
        }
    }
    private IEnumerator FeedBack()
    {
        _isFeedingBack = true;
        Vector2 knockbackDir = (transform.position - _playerPos.position).normalized;
        _knockbackVelocity = knockbackDir * _force;
        _rb.AddForce(knockbackDir * _force, ForceMode2D.Impulse);
        Physics2D.IgnoreLayerCollision(10, 10, true);
        NewColor.a = 0.5f;
        _sr.color = NewColor;

        yield return new WaitForSeconds(_damageTime);

        NewColor.a = 1.0f;
        _sr.color = NewColor;
        _isFeedingBack = false;
        Physics2D.IgnoreLayerCollision(10, 10, false);
    }
}

