using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    private Animator anime;
    private float power = 0.0f;
    [SerializeField] private GameObject burstPrefab;
    [SerializeField] private Transform AttackPoint;
    [SerializeField] private GameObject UnseenBlade;
    [SerializeField] private AudioSource _normalAttack;
    [SerializeField] private AudioSource _chargeAttack;
    [SerializeField] private AudioSource _chargeReady;
    [SerializeField] private Animator _swordAnimator;
    public BoxCollider2D _hitBox;

    private bool _canPlay;
    void Start()
    {
        _canPlay = true;
        anime = gameObject.GetComponent<Animator>();
    }
    void Update()
    {
        Vector3 display = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 vector = Input.mousePosition - display;
        transform.LookAt(vector);
        Fire(vector);
    }
    private void Fire(Vector2 vector)
    {
        if (Input.GetMouseButton(0))
        {
            power += Time.deltaTime;          
        }
        if(power >= 0.5f)
        {
            _swordAnimator.SetBool("DoAttack", false);
            _swordAnimator.SetBool("GetCharge",true);
        }
        if (power >= 0.49f && power <= 0.50f)
        {          
            _chargeReady.Play();
        }
        if (power > 0 && Input.GetMouseButtonUp(0))
        {

            _swordAnimator.SetBool("GetCharge", false);
            _swordAnimator.SetBool("DoAttack", true);
            if (power < 0.5f)
            {
                if (_canPlay == false)
                {
                    return;
                }
                else
                {
                    _normalAttack.PlayOneShot(_normalAttack.clip);
                    anime.SetTrigger("Attack");
                    _canPlay = false;
                }
            }
            else if (power >= 0.5f)
            {
                _chargeAttack.PlayOneShot(_chargeAttack.clip);
                anime.SetTrigger("Charge Attack");
                Instantiate(burstPrefab, AttackPoint.position, AttackPoint.rotation);
            }
            power = 0.0f;
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

    public void IsEnd()
    {
        _canPlay = true;
    }
    public void GetStart()
    {
        _canPlay = false;
    }
}
