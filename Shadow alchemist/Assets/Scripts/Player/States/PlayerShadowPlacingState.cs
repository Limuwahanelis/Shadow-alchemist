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
        _context.shadowControl.PlacingShadowDespawned += ForcedShadowDespawn;
    }
    public override void Attack()
    {
        if(_context.shadowControl.PlaceShadow()) ChangeState(PlayerShadowControlState.StateType);

    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        _context.shadowControl.DespawnShadow();
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void InterruptState()
    {
     
    }
    private void ForcedShadowDespawn()
    {
        _context.shadowControl.PlacingShadowDespawned -= ForcedShadowDespawn;
        ChangeState(PlayerShadowControlState.StateType);
    }
}