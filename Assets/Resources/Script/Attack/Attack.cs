using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    private Animator _anime;
    private float _power;
    private float _chargePoint = 0.7f;
    private float _chargeAnimePoint = 0.2f;
    [SerializeField] private GameObject _burstPrefab;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private AudioSource _normalAttack;
    [SerializeField] private AudioSource _chargeAttack;
    [SerializeField] private AudioSource _chargeReady;
    [SerializeField] private Animator _swordAnimator;
    public BoxCollider2D _hitBox;

    private bool _canPlay;
    private PlayerMovement _playerMovement;

    [SerializeField] private ParticleSystem _chargeParticle;
    private bool chargeParticlePlayed = false;
    [SerializeField] private ParticleSystem _slashParticle;
    void Start()
    {
        _power = 0.0f;
        _canPlay = true;
        _anime = gameObject.GetComponent<Animator>();
        _playerMovement = gameObject.GetComponentInParent<PlayerMovement>();
    }
    void Update()
    {
        if (GameStateManager.instance.CurrentState != GameState.Playing)
        {
            _swordAnimator.speed = 0.0f;
            return;
        }
        else
        {
            _swordAnimator.speed = 1.0f;
        }
        HandleRotation();
        Fire();
    }
    private void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            _power += Time.deltaTime;          
        }
        if (_power >= _chargeAnimePoint && !chargeParticlePlayed)
        {
            _chargeParticle.Play();
            chargeParticlePlayed = true;           
        }
        if (Mathf.Abs(_power - _chargePoint) <= 0.02f)
        {           
            _swordAnimator.SetBool("DoAttack", false);
            _swordAnimator.SetBool("GetCharge", true);
            _chargeReady.Play();
        }
        if (_power > 0 && Input.GetMouseButtonUp(0))
        {
            _chargeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            chargeParticlePlayed = false;

            _swordAnimator.SetBool("GetCharge", false);
            _swordAnimator.SetBool("DoAttack", true);
            if (_power < _chargePoint)
            {
                if (_canPlay == false)
                {
                    return;
                }
                else
                {
                    _normalAttack.PlayOneShot(_normalAttack.clip);
                    _anime.SetTrigger("Attack");
                    _canPlay = false;
                }
            }
            else if (_power >= _chargePoint)
            {
                _chargeAttack.PlayOneShot(_chargeAttack.clip);
                _anime.SetTrigger("Charge Attack");
                Instantiate(_burstPrefab, _attackPoint.position, _attackPoint.rotation);
            }
            _power = 0.0f;
        }
    }
    public void EnableCollider()
    {
        _hitBox.enabled = true;
    }
    public void DisableCollider()
    {
        _hitBox.enabled = false;
    }
    public void GetStart()
    {
        _canPlay = false;
    }
    public void IsEnd()
    {
        _canPlay = true;
    }

    public void SlashEffectStart()
    {
        _playerMovement.StartAttack();
        _slashParticle.Play();
    }
    public void SlashEffectEnd()
    {
        _playerMovement.EndAttack();
        _slashParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    private void HandleRotation()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
