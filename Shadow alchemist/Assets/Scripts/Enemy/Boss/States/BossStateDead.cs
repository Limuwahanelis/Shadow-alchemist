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
        _context = (BossContext)context;
        base.SetUpState(context);
        _context.coroutineHolder.StartCoroutine(HelperClass.DelayedFunction(_context.animMan.GetAnimationLength("teleport hide"), () =>
        {
            _context.bossCollider.enabled = false;
            _context.lanceCollider.enabled = false;
        }));
       
        _context.animMan.PlayAnimation("teleport hide");

    }

    public override void InterruptState()
    {
     
    }
}