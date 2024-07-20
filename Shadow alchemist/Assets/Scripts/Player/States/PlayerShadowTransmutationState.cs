using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowTransmutationState : PlayerState
{
    public static Type StateType { get => typeof(PlayerShadowTransmutationState); }
    public PlayerShadowTransmutationState(GetState function) : base(function)
    {
    }

    public override void Update()
    {

    }
    public override void Move(Vector2 direction)
    {
        if (direction == Vector2.zero) return;
        _context.shadowControl.Shadow.Transmutate(direction);
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void Jump()
    {
        _context.shadowControl.Shadow.RevertTransmutation();
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
    }

    public override void InterruptState()
    {
     
    }
}