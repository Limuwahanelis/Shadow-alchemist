using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateCastSidesMissiles : EnemyState
{
    public static Type StateType { get => typeof(BossStateCastSidesMissiles); }
    private BossContext _context;

    private float _animationTime;
    private float _time = 0;
    public BossStateCastSidesMissiles(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if (_time > _animationTime)
        {
            _context.leftMissilesSpawner.FireMissiles();
            _context.rightMissilesSpawner.FireMissiles();
            ChangeState(BossStateIdle.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _animationTime = _context.animMan.GetAnimationLength("Cast missiles");
        _context.animMan.PlayAnimation("Cast missiles");
        _time = 0;

    }

    public override void InterruptState()
    {

    }
}