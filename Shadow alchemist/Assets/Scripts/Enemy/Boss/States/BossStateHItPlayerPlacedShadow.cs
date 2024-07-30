using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateHItPlayerPlacedShadow : EnemyState
{
    public static Type StateType { get => typeof(BossStateHItPlayerPlacedShadow); }
    private BossContext _context;
    public BossStateHItPlayerPlacedShadow(GetState function) : base(function)
    {
    }

    public override void Update()
    {

    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
    }

    public override void InterruptState()
    {
     
    }
}