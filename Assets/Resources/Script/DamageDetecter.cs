using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DamageDetecter : MonoBehaviour
{
    [SerializeField] private float _knockbackRadius;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private LayerMask _enemyLayer;
    private ImpactParticle impactParticle;

    private void Awake()
    {
        impactParticle = GetComponent<ImpactParticle>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PlayerHP.instance._isReady) return;

        if (collision.CompareTag("Enemy") && PlayerHP.instance._isReady)
        {
            PlayerHP.instance._isEnemy = true;
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            PlayerHP.instance._damage = enemyController._atk;

            KnockbackEnemies();
            Destroy(collision.gameObject);
        }
    }
    private void KnockbackEnemies()
    {
        impactParticle.TriggerParticle();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _knockbackRadius, _enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Vector2 diff = hit.transform.position - transform.position;
                float distance = diff.magnitude;
                Vector2 direction = diff.normalized;

                float forceFactor = 1f - (distance / _knockbackRadius); 
                float finalForce = _knockbackForce * Mathf.Clamp(forceFactor,0,1);

                enemy.Knockback(direction * finalForce);
            }
        }

    }
}
