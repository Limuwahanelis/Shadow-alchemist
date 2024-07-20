using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowControlState : PlayerState
{
    // attack - place
    // jump - move
    // control - transmuate
   // private bool _isTransmutatingShadow = false;
    //private bool _isMovingShadow = false;
    public static Type StateType { get => typeof(PlayerShadowControlState); }
    public PlayerShadowControlState(GetState function) : base(function)
    {
    }

    public override void Update()
    {

    }
    public override void Move(Vector2 direction)
    {

    }
    public override void Attack()
    {
        ChangeState(PlayerShadowPlacingState.StateType);
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        if(controlInput==PlayerInputHandler.ShadowControlInputs.TRANSMUTATE)
        {
            ChangeState(PlayerShadowTransmutationState.StateType);
        }
        else ChangeState(PlayerIdleState.StateType);// _isMovingShadow = !_isMovingShadow;

    }
    public override void Jump()
    {
        ChangeState(PlayerShadowMovingState.StateType);

    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        //_isTransmutatingShadow = false;
        //if(_context.shadowControl.Shadow==null) ChangeState(PlayerIdleState.StateType);
    }

    public override void InterruptState()
    {

    }
}