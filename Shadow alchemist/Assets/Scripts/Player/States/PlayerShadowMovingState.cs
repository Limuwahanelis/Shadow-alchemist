using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowMovingState : PlayerState
{
    public static Type StateType { get => typeof(PlayerShadowMovingState); }
    public PlayerShadowMovingState(GetState function) : base(function)
    {
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
    }
    public override void Update()
    {

    }
    public override void Move(Vector2 direction)
    {
        _context.shadowControl.Shadow.MoveShadow(2f, direction);
    }
    public override void Attack()
    {
        
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void Jump()
    {
        _context.shadowControl.Shadow.RevertMove();
    }
    public override void InterruptState()
    {
     
    }
}