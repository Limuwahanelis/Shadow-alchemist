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
    private float _stunDuration = 15f;
    public BossStateHitPlayerPlacedShadow(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if(_time > _crushAnimLength) 
        {
            _context.animMan.PlayAnimation("Kneel loop");
            _time = 0;
        }
        if(_time> _stunDuration) 
        {
            // change state
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _crushAnimLength = _context.animMan.GetAnimationLength("Shadow crash");
        _context.animMan.PlayAnimation("Shadow crash");
        _time = 0;
    }
    public override void Hit(DamageInfo damageInfo)
    {
        if(damageInfo.damageType==HealthSystem.DamageType.SHADOW_SPIKE)
        {
            // ADD hit from spikes
        }
    }
    public override void InterruptState()
    {
     
    }
}