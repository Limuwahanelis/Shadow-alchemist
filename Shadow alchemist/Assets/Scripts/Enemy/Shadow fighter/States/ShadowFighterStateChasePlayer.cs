using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateChasePlayer : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateChasePlayer); }
    private ShadowFighterContext _context;
    public ShadowFighterStateChasePlayer(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        if (Vector2.Distance(_context.enemyTransform.position, _context.playerTransform.position) > _context.minPlayerRange)
        {
            if (_context.enemyTransform.position.x - _context.originShadow.ShadowBounds.min.x > _context.distanceFromShadowBounds)
            {
                if (_context.enemyTransform.position.x > _context.playerTransform.position.x) _context.movement.Move(Vector2.left);
            }
            if (_context.originShadow.ShadowBounds.max.x - _context.enemyTransform.position.x > _context.distanceFromShadowBounds)
            {
                if (_context.enemyTransform.position.x < _context.playerTransform.position.x) _context.movement.Move(Vector2.right);
            }
            if (Vector2.Distance(_context.enemyTransform.position, _context.playerTransform.position) > _context.maxPlayerEngageDistance) 
            {
                if (_context.patrolPoints.Count == 1) ChangeState(ShadowFighterStateGoBackToSpawn.StateType);
                else if (_context.patrolPoints.Count > 1) ChangeState(ShadowFighterStatePatrol.StateType);
                else ChangeState(ShadowFighterStateIdle.StateType);
            }
        }
        else
        {
            
             ChangeState(ShadowFighterStateAttacking.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
    }

    public override void InterruptState()
    {
     
    }
}