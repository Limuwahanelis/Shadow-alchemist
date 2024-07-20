using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowPlacingState : PlayerState
{
    public static Type StateType { get => typeof(PlayerShadowPlacingState); }
    public PlayerShadowPlacingState(GetState function) : base(function)
    {
    }

    public override void Update()
    {

    }

    public override void Move(Vector2 direction)
    {
        _context.shadowControl.ShadowToPlace.Move(direction);
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.shadowControl.SpawnShadow();
    }
    public override void Attack()
    {
        _context.shadowControl.PlaceShadow();
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        _context.shadowControl.DespawnShadow();
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void InterruptState()
    {
     
    }
}