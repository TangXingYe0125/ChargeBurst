using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghost : EnemyController
{
    private float _skillCoolDown = 4.0f;
    private bool _isUsingSkill = false;
    private bool _isOnCooldown = false;
    private int _originalLayer;
    private string _invincibleLayerName = "GhostInvincible";

    protected override void Start()
    {
        base.Start();
        gameObject.layer = _originalLayer;
    }
    void Update()
    {
        if (!_isUsingSkill && !_isOnCooldown && (_state != EnemyState.Hurt))
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
}
