using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossStateIdle : EnemyState
{
    public static Type StateType { get => typeof(BossStateIdle); }
    private BossContext _context;
    private float _idleTime=4;
    private float _time;
    public BossStateIdle(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if(_time > _idleTime) 
        {

            if(_context.currentPhase==BossController.BossPhase.MISSILES_TOP)
            {
                _context.currentPhase = BossController.BossPhase.NORMAL_ATTACK;
                // player is on left side of stage
                if (_context.playerTransform.position.x < 11) _context.indexOfTeleportPos = 4;
                else  _context.indexOfTeleportPos = 3;
                ChangeState(BossStateTeleport.StateType);
                // teleport to player
            }
            else if(_context.currentPhase==BossController.BossPhase.MISSLES_SIDES)
            {
                _context.currentPhase = BossController.BossPhase.CHARGE;
                if (_context.playerTransform.position.x < 11) _context.indexOfTeleportPos = 2;
                else _context.indexOfTeleportPos = 0;
                ChangeState(BossStateTeleport.StateType);
            }
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.animMan.PlayAnimation("Idle");
        _time = 0;
    }

    public override void InterruptState()
    {
     
    }
}