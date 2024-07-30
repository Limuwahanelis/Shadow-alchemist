using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateCastTopMissiles : EnemyState
{
    public static Type StateType { get => typeof(BossStateCastTopMissiles); }
    private BossContext _context;

    private float _animationTime;
    private float _time = 0;
    private bool _hasFiredMissiles=false;
    public BossStateCastTopMissiles(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if (!_hasFiredMissiles)
        {
            if (_time > _animationTime)
            {
                _context.topMissilesSpawner.FireMissiles();
                _hasFiredMissiles = true;
                ChangeState(BossStateIdle.StateType);
            }
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _animationTime = _context.animMan.GetAnimationLength("Cast missiles");
        _context.animMan.PlayAnimation("Cast missiles");
        _time = 0;
        _hasFiredMissiles = false;

    }

    public override void InterruptState()
    {
     
    }
}