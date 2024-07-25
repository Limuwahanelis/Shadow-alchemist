using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateGoBackToSpawn : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateGoBackToSpawn); }
    private ShadowFighterContext _context;
    public ShadowFighterStateGoBackToSpawn(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        if (Vector2.Distance(_context.enemyTransform.position, _context.patrolPoints[0].position) > 0.4f)
        {
            if (_context.enemyTransform.position.x > _context.patrolPoints[0].position.x) _context.movement.Move(Vector2.left);
            if (_context.enemyTransform.position.x < _context.patrolPoints[0].position.x) _context.movement.Move(Vector2.right);
        }
        else ChangeState(ShadowFighterStateIdle.StateType);
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.frontPlayerDetection.OnPlayerDetectedUnity.AddListener(PlayerDetected);
    }
    private void PlayerDetected()
    {
        _context.frontPlayerDetection.OnPlayerDetectedUnity.RemoveListener(PlayerDetected);
        ChangeState(ShadowFighterStateChasePlayer.StateType);
    }
    public override void InterruptState()
    {
        _context.frontPlayerDetection.OnPlayerDetectedUnity.RemoveListener(PlayerDetected);
    }
}