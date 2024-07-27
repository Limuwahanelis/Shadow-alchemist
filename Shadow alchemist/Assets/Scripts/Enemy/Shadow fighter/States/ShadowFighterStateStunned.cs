using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateStunned : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateStunned); }
    private ShadowFighterContext _context;
    private float _time;
    public ShadowFighterStateStunned(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if(_time>=10)
        {
            _context.WaitFrameAndPerformFunction(() => { ChangeState(ShadowFighterStateIdle.StateType); });
            
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.animMan.SetAnimator(false);
        _context.combat.SetStunViusals(true);
        _context.weakendStatus.Status=EnemyWeakendStatus.WeakenStatus.STUNNED;
    }

    public override void InterruptState()
    {
        _context.animMan.SetAnimator(true);
        _context.combat.SetStunViusals(false);
    }
}