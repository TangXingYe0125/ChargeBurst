using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected float _force;
    [SerializeField] private float _damageTime;
    private bool _isFeedingBack;
    protected enum EnemyState { Idle, Chase, Wait, Return, Hurt }
    protected EnemyState _state;

    protected Transform _playerPos;
    [SerializeField] protected float _speed;
    [SerializeField] protected int _hp;
    public int _atk;
    protected Rigidbody2D _rb;

    [SerializeField] protected Zone _zone;
    protected Vector3 _startPos;
    protected Vector3 _lastVelocity;
    protected float _waitTime = 1.0f;
    protected float _waitT = 0.0f;

    protected float _damageCooldown = 0.2f;
    protected float _lastHitTime;

    [SerializeField] protected Animator _animator;
    public bool _isKnockedBack;

    protected int _originalLayer;
    protected string _invincibleLayerName = "EnemyInvincible";
    private void Awake()
    {
        if(_zone == null)
        {
            _zone = GameObject.FindGameObjectWithTag("Zone").GetComponent<Zone>();
        }
    }
    protected virtual void Start()
    {
        _startPos = transform.position;
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _state = EnemyState.Idle;
        _isFeedingBack = false;
        _isKnockedBack = false;
        _lastHitTime = -1f;
        _originalLayer = gameObject.layer;
    }

    private void FixedUpdate()
    {
        if (GameStateManager.instance.CurrentState != GameState.Playing)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        Track();
        TurnDirection();
    }
    protected virtual IEnumerator WaitThenReturn()
    {
        _state = EnemyState.Wait;
        yield return new WaitForSeconds(_waitTime);
        _state = EnemyState.Return;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - _lastHitTime < _damageCooldown) return;

        if (collision.CompareTag("Burst"))
        {
            _hp -= 3;
            EnterHurtState();
        }
        else if (collision.CompareTag("Sword"))
        {
            _hp -= 1;
            EnterHurtState();
        }
    }
    protected virtual void Track()
    {
        if (_isKnockedBack || _state == EnemyState.Hurt) return;

        if (_hp <= 0)
        {
            _rb.velocity = Vector2.zero;
            Kills.instance._kills++;

            _animator.SetTrigger("Die");

            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            return;
        }
        if (_zone._isPlayerGetIn)
        {
            _state = EnemyState.Chase;
        }

        switch (_state)
        {
            case EnemyState.Idle:

                break;

            case EnemyState.Chase:
                if (_zone._isPlayerGetIn && !_isFeedingBack)
                {
                    Vector2 chaseDir = (_playerPos.position - transform.position).normalized;
                    _rb.velocity = chaseDir * _speed;
                    _waitT = 0.0f;
                }
                else
                {
                    _lastVelocity = _rb.velocity;
                    StartCoroutine(WaitThenReturn());
                }
                break;

            case EnemyState.Wait:
                _waitT += Time.fixedDeltaTime;
                _rb.velocity = Vector3.Lerp(_lastVelocity, Vector3.zero, _waitT / _waitTime);
                break;

            case EnemyState.Return:
                if (Vector2.Distance(transform.position, _startPos) > 0.1f)
                {
                    Vector2 returnDir = (_startPos - transform.position).normalized;
                    _rb.velocity = returnDir * _speed;
                }
                else
                {
                    _rb.velocity = Vector2.zero;
                    _state = EnemyState.Idle;
                }
                break;
        }
    }
    private IEnumerator FeedBack()
    {
        _isFeedingBack = true;
        Vector2 knockbackDir = (transform.position - _playerPos.position).normalized;
        _rb.AddForce(knockbackDir * _force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_damageTime);
        _isFeedingBack = false;
    }
    protected virtual void TurnDirection()
    {
        if (transform.position.x >= _playerPos.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    public void Knockback(Vector2 force)
    {
        _isKnockedBack = true;
        _rb.velocity = Vector2.zero;
        _rb.AddForce(force, ForceMode2D.Impulse);
        StartCoroutine(RecoverFromKnockback());
    }
    private IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(1.0f);
        _isKnockedBack = false;
    }

    protected virtual void EnterHurtState()
    {
        if (_state == EnemyState.Hurt) return;
        _state = EnemyState.Hurt;

        gameObject.layer = LayerMask.NameToLayer(_invincibleLayerName);
        _animator.SetTrigger("Hit");
        StartCoroutine(FeedBack());
        _lastHitTime = Time.time;

        StartCoroutine(RecoverFromHurt());
    }
    protected virtual IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.layer = _originalLayer;
        _state = EnemyState.Chase;
    }
}

