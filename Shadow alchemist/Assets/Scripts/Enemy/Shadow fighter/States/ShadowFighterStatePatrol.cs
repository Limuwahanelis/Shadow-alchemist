using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShadowFighterStatePatrol : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStatePatrol); }
    private ShadowFighterContext _context;
    private bool _isGoingBack = false;
    public ShadowFighterStatePatrol(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        if (MathF.Abs(_context.enemyTransform.position.x- _context.patrolPoints[_context.patrolPointIndex].position.x)>0.3f)
        {
            if (_context.enemyTransform.position.x - _context.patrolPoints[_context.patrolPointIndex].position.x <= 0) _context.movement.Move(Vector2.right);
            else _context.movement.Move(Vector2.left);
        }
        else
        {
            if (_isGoingBack) _context.patrolPointIndex--;
            else _context.patrolPointIndex++;
            if(_context.patrolPointIndex==_context.patrolPoints.Count)
            {
                _isGoingBack = true;
                _context.patrolPointIndex-=2;
            }
            else if(_context.patrolPointIndex==-1)
            {
                _isGoingBack=false;
                _context.patrolPointIndex+=2;
            }
            ChangeState(ShadowFighterStateIdle.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.animMan.PlayAnimation("Walk");
        _context.frontPlayerDetection.OnPlayerDetectedUnity.AddListener(PlayerDetected);
    }
    private void PlayerDetected()
    {
        
        ChangeState(ShadowFighterStateChasePlayer.StateType);
    }
    public override void InterruptState()
    {
        _context.frontPlayerDetection.OnPlayerDetectedUnity.RemoveListener(PlayerDetected);
    }
}