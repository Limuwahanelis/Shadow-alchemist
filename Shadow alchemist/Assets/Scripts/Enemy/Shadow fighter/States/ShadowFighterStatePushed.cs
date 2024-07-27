using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStatePushed : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStatePushed); }
    private ShadowFighterContext _context;
    private bool _isOffGround = false;
    public ShadowFighterStatePushed(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        if (!_context.enemyChecks.IsTouchingGround && !_isOffGround)
        {
                _isOffGround=true;
        }
        if(_context.enemyChecks.IsTouchingGround && _isOffGround)
        {
            _isOffGround=false;
                _context.WaitFrameAndPerformFunction(() => { ChangeState(ShadowFighterStateIdle.StateType); });
        }
    }
    public void SetUpState(EnemyContext context,DamageInfo damageInfo)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.animMan.SetAnimator(false);
        _context.combat.SetStunViusals(true);
        _context.movement.Stop();
        // sub result - <0 mewans palyer is on right, else its on left. mult result - <0 player is in front, else player is behind
        if ((_context.enemyTransform.position.x - damageInfo.dmgPosition.x) * ((int)_context.movement.FlipSide) > 0) _context.movement.Push(-1);
        else _context.movement.Push();
        _isOffGround = false;
    }
    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.animMan.SetAnimator(false);
        _context.combat.SetStunViusals(true);
        _context.movement.Stop();
        _context.movement.Push();
        _isOffGround = false;
    }

    public override void InterruptState()
    {
        _context.animMan.SetAnimator(true);
        //_context.combat.SetStunViusals(false);
    }
}