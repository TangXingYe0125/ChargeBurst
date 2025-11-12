using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghost : EnemyController
{
    private float _skillCoolDown = 4.0f;
    private bool _isUsingSkill = false;
    private bool _isOnCooldown = false;

    private float _lastHurtTime = -999f;
    private float _hurtDelay = 4.0f;
    void Update()
    {
        if (!_isUsingSkill && !_isOnCooldown && _state != EnemyState.Hurt && (Time.time - _lastHurtTime >= _hurtDelay))
        {
            StartCoroutine(UseSkill());
        }
    }
    private IEnumerator UseSkill()
    {
        _isUsingSkill = true;
        _isOnCooldown = true;
        _animator.SetTrigger("Skill");

        gameObject.layer = LayerMask.NameToLayer(_invincibleLayerName);

        yield return new WaitUntil(() => !_isUsingSkill);

        yield return new WaitForSeconds(_skillCoolDown);
        _isOnCooldown = false;
    }

    public void OnSkillEnd()
    {
        gameObject.layer = _originalLayer;
        _isUsingSkill = false;
    }

    protected override void EnterHurtState()
    {
        base.EnterHurtState();
        _lastHurtTime = Time.time;
    }
}
