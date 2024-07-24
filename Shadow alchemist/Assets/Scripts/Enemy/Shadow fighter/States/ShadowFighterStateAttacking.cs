using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateAttacking : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateAttacking); }
    private ShadowFighterContext _context;
    public ShadowFighterStateAttacking(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        if (Vector2.Distance(_context.enemyTransform.position, _context.playerTransform.position) > _context.maxPlayerRange)
        {
            ChangeState(ShadowFighterStateChasePlayer.StateType);
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