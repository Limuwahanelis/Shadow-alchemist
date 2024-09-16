using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchingWalkingState : PlayerState
{
    public static Type StateType { get => typeof(PlayerCrouchingWalkingState); }
    public PlayerCrouchingWalkingState(GetState function) : base(function)
    {
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.animationManager.PlayAnimation("Crouch walk");
    }
    public override void Update()
    {
        PerformInputCommand();
        if(!_context.checks.IsNearCeiling)
        {
            _context.collisions.SetCrouchColliiers(false);
            _context.collisions.SetNormalColliders(true);
            ChangeState(PlayerWalkingState.StateType);
        }
        if (!_context.checks.IsOnGround)
        {
            _context.collisions.SetCrouchColliiers(false);
            _context.collisions.SetNormalColliders(true);
            ChangeState(PlayerInAirState.StateType);
        }
    }
    public override void Push()
    {
        ChangeState(PlayerCrouchedPushedState.StateType);
    }
    public override void Move(Vector2 direction)
    {
        if (direction.x == 0)
        {
            ChangeState(PlayerCrouchingIdleState.StateType);
        }
        _context.playerMovement.Move(direction.x);
    }
    public override void InterruptState()
    {

    }
}