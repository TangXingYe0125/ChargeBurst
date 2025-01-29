using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

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

    void Start()
    {
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
        if (power >= 0.49f && power <= 0.50f)
        {
            //_chargeReady.PlayOneShot(_chargeReady.clip);
            _chargeReady.Play();
        }
        if (power > 0 && Input.GetMouseButtonUp(0))
        {
            if (power < 0.5f)
            {
                _normalAttack.PlayOneShot(_normalAttack.clip);
                anime.SetTrigger("Attack");
                Instantiate(UnseenBlade, AttackPoint.position, AttackPoint.rotation);
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
}
