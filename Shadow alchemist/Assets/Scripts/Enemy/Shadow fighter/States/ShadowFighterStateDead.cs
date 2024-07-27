using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateDead : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateDead); }
    private ShadowFighterContext _context;
    private float _time;
    private float _deathTime;
    public ShadowFighterStateDead(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if ((_time>_deathTime))
        {
             _context.DestroyItself();
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;
        _context.animMan.PlayAnimation("Death");
        _deathTime = _context.animMan.GetAnimationLength("Death");
        _context.coroutineHolder.StopAllCoroutines();
    }

    public override void InterruptState()
    {
     
    }
}