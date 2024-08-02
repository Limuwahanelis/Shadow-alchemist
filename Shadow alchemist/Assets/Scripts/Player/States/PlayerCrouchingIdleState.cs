using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchingIdleState : PlayerState
{
    public static Type StateType { get => typeof(PlayerCrouchingIdleState); }
    public PlayerCrouchingIdleState(GetState function) : base(function)
    {
    }
    public override void Move(Vector2 direction)
    {
        if (Math.Abs(direction.x) > 0) ChangeState(PlayerCrouchingWalkingState.StateType);
    }
    public override void Update()
    {
        PerformInputCommand();
        if (!_context.checks.IsNearCeiling)
        {
            _context.collisions.SetCrouchColliiers(false);
            _context.collisions.SetNormalColliders(true);
            ChangeState(PlayerIdleState.StateType);
        }

    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.collisions.SetCrouchColliiers(true);
        _context.collisions.SetNormalColliders(false);
        _context.animationManager.PlayAnimation("Crouch idle");
    }

    public override void InterruptState()
    {

    }
}