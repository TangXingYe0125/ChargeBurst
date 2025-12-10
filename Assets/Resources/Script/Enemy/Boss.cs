using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyController
{
    [SerializeField] private BossHPBar _bossHPBar;
    private CircleCollider2D _circleCollider;
    private CapsuleCollider2D _capsuleCollider;
    private BulletShooter bulletShooter;
    private bool _isHurt;
    private float _originalDamageCooldown;
    [SerializeField] private BladeArray bladeArray;
    private bool _isDead = false;
    [SerializeField] private Animator _dieBodyAnimator;

    protected override void Start()
    {
        base.Start();
        _originalDamageCooldown = _damageCooldown;
        _circleCollider = GetComponent<CircleCollider2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _circleCollider.enabled = true;
        _capsuleCollider.enabled = false;
        bulletShooter = GetComponent<BulletShooter>();

        bladeArray.OnStartBossAttack += () =>
        {
            _damageCooldown = 0f;   
        };
        bladeArray.OnEndBossAttack += () =>
        {
            _damageCooldown = _originalDamageCooldown; 
        };
    }

    private void Update()
    {
        if (_hp <= 0 && !_isDead)
        {
            _isDead = true;
            StartCoroutine(BossDeathSequence());
        }
    }
    protected override void Track()
    {

    }
    protected virtual void CheckPhaseChange(int oldHP, int newHP)
    {
        if (oldHP > bladeArray.ctrlTriggerHp && newHP <= bladeArray.ctrlTriggerHp)
        {
            _circleCollider.enabled = false;
            _capsuleCollider.enabled = true;
            _animator.SetTrigger("Ph2");
            Destroy(bulletShooter);
        }
    }

    protected override void TurnDirection()
    {
        if (transform.position.x >= _playerPos.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - _lastHitTime < _damageCooldown) return;

        int _damage = 0;
        if (collision.CompareTag("Burst"))
        {
            _damage = 3;
        }
        else if (collision.CompareTag("Sword"))
        {
            _damage = 1;
        }
        else if (collision.CompareTag("Blade"))
        {
            _damage = 1;
        }
        if (_damage > 0)
        {
            int oldHP = _hp; 
            _hp -= _damage;
            _bossHPBar.TakeDamage(_damage);
            StartCoroutine(BossHurt());
            CheckPhaseChange(oldHP, _hp);
        }

        if (!collision.CompareTag("PlayerBody")) return;
        if (Time.time - _lastHitTime < _damageCooldown) return;

        _lastHitTime = Time.time;

        Rigidbody2D playerRb = collision.GetComponentInParent<Rigidbody2D>();

        PlayerMovement pm = collision.GetComponentInParent<PlayerMovement>();

        Vector2 dir = (collision.transform.position - transform.position).normalized;

        StartCoroutine(KnockbackPlayer(playerRb, pm, dir));

    }
    private IEnumerator BossHurt()
    {
        if (_isHurt)
        {
            yield break;
        }
        _isHurt = true;
        gameObject.layer = LayerMask.NameToLayer(_invincibleLayerName);
        _lastHitTime = Time.time;
        yield return new WaitForSeconds(0.5f);
        gameObject.layer = _originalLayer;
        _isHurt =false;
    }
    private IEnumerator KnockbackPlayer(Rigidbody2D playerRb, PlayerMovement pm, Vector2 dir)
    {
        playerRb.freezeRotation = true;

        playerRb.velocity = Vector2.zero;
        playerRb.AddForce(dir * _force, ForceMode2D.Impulse);

        if (pm != null)
        {
            pm._isKnocked = true;
        }

        yield return new WaitForSeconds(0.5f);

        playerRb.freezeRotation = false;

        if (pm != null)
        {
            pm._isKnocked = false;
        }
    }

    private IEnumerator BossDeathSequence()
    {
        _dieBodyAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(0.4f);
        Destroy(_animator);
        var sr = _animator.GetComponent<SpriteRenderer>();
        sr.sprite = null;

        yield return new WaitForSeconds(1.0f);

        GameStateManager.instance.SetState(GameState.Clear);
    }
}
