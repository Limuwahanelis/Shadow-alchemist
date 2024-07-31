using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateDead : EnemyState
{
    public static Type StateType { get => typeof(BossStateDead); }
    private BossContext _context;
    public BossStateDead(GetState function) : base(function)
    {
    }

    public override void Update()
    {

    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.animMan.PlayAnimation("teleport hide");
    }

    public override void InterruptState()
    {
     
    }
}