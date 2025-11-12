using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyController
{
    [SerializeField] private BossHPBar _bossHPBar;
    protected override void Track()
    {

    }
    protected virtual void CheckPhaseChange(int oldHP, int newHP)
    {
        if (oldHP > 30 && newHP <= 30)
        {
            _animator.SetTrigger("Ph2");
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
        int _damage = 0;
        if (collision.CompareTag("Burst"))
        {
            _damage = 3;
        }
        else if (collision.CompareTag("Sword"))
        {
            _damage = 1;
        }
        if (_damage > 0)
        {
            int oldHP = _hp; 
            _hp -= _damage;
            _bossHPBar.TakeDamage(_damage);

            CheckPhaseChange(oldHP, _hp);
        }
    }
}
