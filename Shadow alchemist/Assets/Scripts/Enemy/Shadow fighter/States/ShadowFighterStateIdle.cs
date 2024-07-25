using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateIdle : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateIdle); }
    private ShadowFighterContext _context;

    private float _idletime;
    private float _time;
    public ShadowFighterStateIdle(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        
        if(_time >= _idletime)
        {
            if (Vector2.Distance(_context.enemyTransform.position, _context.playerTransform.position) < _context.minPlayerRange)
            {
                ChangeState(ShadowFighterStateAttacking.StateType);
            }
             else if (_context.patrolPoints.Count > 1) 
            {
                ChangeState(ShadowFighterStatePatrol.StateType);
            }
            
        }
        _time += Time.deltaTime;
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.animMan.PlayAnimation("Idle");
        _idletime = 0.5f;//_context.animMan.GetAnimationLength("Idle");
        _time = 0;
        _context.frontPlayerDetection.OnPlayerDetectedUnity.AddListener(PlayerDetected);
    }
    private void PlayerDetected()
    {
        _context.frontPlayerDetection.OnPlayerDetectedUnity.RemoveListener(PlayerDetected);
        ChangeState(ShadowFighterStateChasePlayer.StateType);
    }
    public override void InterruptState()
    {
     
    }
}