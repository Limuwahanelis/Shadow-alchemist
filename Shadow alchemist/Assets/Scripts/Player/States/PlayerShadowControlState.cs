using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowControlState : PlayerState
{
    public static Type StateType { get => typeof(PlayerShadowControlState); }
    public PlayerShadowControlState(GetState function) : base(function)
    {
    }

    public override void Update()
    {

    }
    public override void Move(Vector2 direction)
    {
        _context.shadowControl.Shadow.MoveShadow(3f,direction);
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        if(_context.shadowControl.Shadow==null) ChangeState(PlayerIdleState.StateType);
    }

    public override void InterruptState()
    {
     
    }
}