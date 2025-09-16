using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakEnemyController : MonoBehaviour
{
    private Transform _playerPos;
   [SerializeField] private float _speed;
    public int _hp = 1;
    private Rigidbody2D _rb;
    [SerializeField] private Zone _zone;
    private Vector3 _startPos;
    public bool _shouldWait;

    private void Start()
    {
        _startPos = this.gameObject.transform.position;
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _shouldWait = false;
    }
    private void FixedUpdate()
    {
        if (_hp > 0)
        {
            if (_zone._isPlayerGetIn)
            {
                Vector2 direction = (_playerPos.position - transform.position).normalized;
                _rb.velocity = direction * _speed;
                _shouldWait = false;
            }
            else if (!_zone._isPlayerGetIn)
            {
                _shouldWait = true;
                if(!_shouldWait)
                {
                    if (Vector2.Distance(transform.position, _startPos) > 0.1f)
                    {
                        Vector2 direction = (_startPos - transform.position).normalized;
                        _rb.velocity = direction * _speed;
                    }
                    else
                    {
                        _rb.velocity = Vector2.zero;
                    }
                }
                if (_shouldWait)
                {
                    StartCoroutine(Wait());
                }
                
            }
        }
        else
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
        }
    }
    private IEnumerator Wait()
    {

        yield return new WaitForSeconds(1.0f);
        _shouldWait = false ;

    }
}
