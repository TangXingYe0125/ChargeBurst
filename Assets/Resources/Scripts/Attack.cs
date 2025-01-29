using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Animator anime;
    float power = 0.0f;
    public GameObject burstPrefab;
    public Transform AttackPoint;
    public GameObject UnseenBlade;
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
        if (power > 0 && Input.GetMouseButtonUp(0))
        {
            if (power < 0.8f)
            {
                anime.SetTrigger("Attack");
                Instantiate(UnseenBlade, AttackPoint.position, AttackPoint.rotation);
            }
            else if (power >= 0.8f)
            {
                anime.SetTrigger("Charge Attack");
                Instantiate(burstPrefab, AttackPoint.position, AttackPoint.rotation);
            }
            power = 0.0f;
        }
    }
}
