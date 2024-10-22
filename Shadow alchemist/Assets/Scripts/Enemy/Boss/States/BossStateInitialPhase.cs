using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateInitialPhase : EnemyState
{
    public static Type StateType { get => typeof(BossStateInitialPhase); }
    private BossContext _context;
    public BossStateInitialPhase(GetState function) : base(function)
    {
    }

    public override void Update()
    {
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.indexOfTeleportPos = 1;
        _context.animMan.PlayAnimation("Idle");
        _context.OnFightStarted += StartFight;
    }
    private void StartFight()
    {
        ChangeState(BossStateTeleport.StateType);
    }
    public override void InterruptState()
    {
        _context.OnFightStarted -= StartFight;
    }
}