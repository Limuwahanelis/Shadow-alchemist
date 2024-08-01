using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossStateHitPlayerPlacedShadow : EnemyState
{
    public static Type StateType { get => typeof(BossStateHitPlayerPlacedShadow); }
    private BossContext _context;
    private float _time;
    private float _crushAnimLength;
    private float _stunDuration = 5f;
    private bool _isKneeling = false;
    public BossStateHitPlayerPlacedShadow(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if (!_isKneeling)
        {
            if (_time > _crushAnimLength)
            {
                _isKneeling = true;
                _context.animMan.PlayAnimation("Kneel loop");
                _time = 0;
            }
        }
        if(_time> _stunDuration) 
        {
            _context.weakendStatus.Status = EnemyWeakendStatus.WeakenStatus.NONE;
            _context.sweatDrop.SetActive(false);
            ChangeState(BossStateRecoveryFromShadowCollision.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _crushAnimLength = _context.animMan.GetAnimationLength("Shadow crash");
        _context.animMan.PlayAnimation("Shadow crash");
        _context.weakendStatus.Status = EnemyWeakendStatus.WeakenStatus.WEAKEN;
        _time = 0;
        _stunDuration = 2f;
        _isKneeling = false;
        _context.healthSystem.TakeDamageWithoutNotify(new DamageInfo(20, HealthSystem.DamageType.ENEMY, _context.enemyTransform.position));
        _context.sweatDrop.SetActive(true);
    }
    public override void Hit(DamageInfo damageInfo)
    {
        if(damageInfo.damageType==HealthSystem.DamageType.SHADOW_SPIKE)
        {
            Logger.Log("HIT from spikes");
            _context.sweatDrop.SetActive(false);
            _context.healthSystem.TakeDamageWithoutNotify(new DamageInfo(30, HealthSystem.DamageType.ENEMY, _context.enemyTransform.position));
            _stunDuration += 10f;
            _context.weakendStatus.Status = EnemyWeakendStatus.WeakenStatus.STUNNED;
        }
    }
    public override void InterruptState()
    {
        _context.sweatDrop.SetActive(false);
    }
}