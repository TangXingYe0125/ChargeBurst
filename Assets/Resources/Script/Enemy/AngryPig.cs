using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryPig : EnemyController
{
    [SerializeField] private AnimatorOverrideController _angryPigOverrideController;

    protected override void EnterHurtState()
    {
        base.EnterHurtState();
        _animator.runtimeAnimatorController = _angryPigOverrideController;
        _speed = 3.0f;
        _atk = 2;
    }

}
