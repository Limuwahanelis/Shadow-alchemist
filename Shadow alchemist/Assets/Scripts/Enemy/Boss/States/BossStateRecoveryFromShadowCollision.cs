using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateRecoveryFromShadowCollision : EnemyState
{
    public static Type StateType { get => typeof(BossStateRecoveryFromShadowCollision); }
    private BossContext _context;
    private float _time;
    private float _animationLength;
    public BossStateRecoveryFromShadowCollision(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if(_time > _animationLength) 
        {
            _context.currentPhase=BossController.BossPhase.MISSILES_TOP;
            _context.indexOfTeleportPos = 1;
            ChangeState(BossStateTeleport.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.animMan.PlayAnimation("Get up");
        _animationLength = _context.animMan.GetAnimationLength("Get up");
        _time = 0;
    }

    public override void InterruptState()
    {
     
    }
}