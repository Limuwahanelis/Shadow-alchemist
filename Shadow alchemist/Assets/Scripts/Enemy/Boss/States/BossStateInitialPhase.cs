using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateInitialPhase : EnemyState
{
    public static Type StateType { get => typeof(BossStateInitialPhase); }
    private BossContext _context;
    private float _time = 0;
    private float _timeReuqiredForteleprtUP = 2f;
    public BossStateInitialPhase(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if(_time>=_timeReuqiredForteleprtUP)
        {
            ChangeState(BossStateTeleport.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.indexOfTeleportPos = 1;
        _context.animMan.PlayAnimation("Idle");
    }

    public override void InterruptState()
    {
     
    }
}