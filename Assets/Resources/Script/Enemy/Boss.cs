using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyController
{
    [SerializeField] private BossHPBar _bossHPBar;
    private CircleCollider2D _circleCollider;
    private CapsuleCollider2D _capsuleCollider;
    private BulletShooter bulletShooter;
    [SerializeField]private BossEnemyInstantiate bossEnemyInstantiate;
    private bool _isHurt;
    private float _originalDamageCooldown;
    [SerializeField] private BladeArray bladeArray;
    private bool _isDead = false;
    [SerializeField] private Animator _dieBodyAnimator;
    private AudioSource _bossHit;
    [SerializeField] private float hitSoundCooldown = 0.12f;
    private float _lastHitSoundTime = -999f;
    
    private bool _isEntering = false;
    [SerializeField] private GameObject _fireRing;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _fadeWaitTime;
    [SerializeField] private SpriteRenderer _bossBody;
    protected override void Start()
    {
        base.Start();

        Color c = _bossBody.color;
        c.a = 0f;
        _bossBody.color = c;
        _bossHPBar.gameObject.SetActive(false);
        _bossHit = GetComponent<AudioSource>();
        _originalDamageCooldown = _damageCooldown;
        _circleCollider = GetComponent<CircleCollider2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _circleCollider.enabled = true;
        _capsuleCollider.enabled = false;
        bulletShooter = GetComponent<BulletShooter>();
        bulletShooter.enabled = false;

        bladeArray.OnStartBossAttack += () =>
        {
            _damageCooldown = 0f;   
        };
        bladeArray.OnEndBossAttack += () =>
        {
            _damageCooldown = _originalDamageCooldown;
            _isDead = true;
        };
        _fireRing.SetActive(false);
    }
    private void Update()
    {
        if (_hp <= 0 &&_isDead)
        {
            _isDead = false;
            StartCoroutine(BossDeathSequence());
        }
    }
    private void OnEnable()
    {
        StartCoroutine(BossEntrance());
    }
    protected override void Track()
    {

    }
    protected virtual void CheckPhaseChange(int oldHP, int newHP)
    {
        if (oldHP > bossEnemyInstantiate._instantiateHP && newHP <= bossEnemyInstantiate._instantiateHP)
        {
            StartCoroutine(bossEnemyInstantiate.BossInstantiateEnemy());           
        }

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
        if (_isEntering) return;
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
        else if (collision.CompareTag("Finisher"))
        {
            _damage = 99;
        }
        if (_damage > 0)
        {
            int oldHP = _hp;

            bool isFinisher = collision.CompareTag("Finisher"); 

            if (!isFinisher && _hp - _damage <= 0)
            {
                _hp = 1;
            }
            else
            {
                _hp -= _damage;
            }

            _bossHPBar.TakeDamage(oldHP - _hp);

            if (Time.time - _lastHitSoundTime >= hitSoundCooldown)
            {
                _bossHit.PlayOneShot(_bossHit.clip);
                _lastHitSoundTime = Time.time;
            }

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
        yield return new WaitForSeconds(0.6f);
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

    public IEnumerator BossEntrance()
    {
        yield return new WaitForSeconds(_fadeWaitTime);

        _isEntering = true;
        gameObject.layer = LayerMask.NameToLayer("EntryLayer"); 

        _fireRing.SetActive(true);
        yield return new WaitForSeconds(_fadeWaitTime);

        yield return StartCoroutine(Fade(0.0f, 1.0f, _fadeDuration, _bossBody));

        gameObject.layer = _originalLayer;
        _isEntering = false;
        _bossHPBar.gameObject.SetActive(true);
        yield return new WaitForSeconds(_fadeWaitTime);

        bulletShooter.enabled = true;
    }
    private IEnumerator Fade(float from, float to, float duration, SpriteRenderer sr)
    {
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
            yield return null;
        }
    }
}
