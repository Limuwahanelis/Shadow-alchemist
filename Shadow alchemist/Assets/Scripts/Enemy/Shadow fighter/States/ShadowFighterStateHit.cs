using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateHit : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateHit); }
    private ShadowFighterContext _context;
    public ShadowFighterStateHit(GetState function) : base(function)
    {
    }

    public override void Update()
    {

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