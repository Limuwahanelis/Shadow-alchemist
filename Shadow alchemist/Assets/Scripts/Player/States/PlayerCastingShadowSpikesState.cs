using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastingShadowSpikesState : PlayerState
{
    public static Type StateType { get => typeof(PlayerCastingShadowSpikesState); }
    public PlayerCastingShadowSpikesState(GetState function) : base(function)
    {
    }
    public override void Update()
    {
        PerformInputCommand();
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.playerMovement.StopPlayer();
        if(_context.shadowControl.FullShadow) _context.shadowSpikeSkill.SetFullShadow(_context.shadowControl.FullShadow);
        else _context.shadowSpikeSkill.SetOriginShadow(_context.shadowControl.Shadow);
        _context.shadowSpikeSkill.CastSpikes(_context.playerMovement.FlipSide);
        _context.animationManager.PlayAnimation("Shadow cast");
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        _context.shadowSpikeSkill.StopCastingSpikes();
        ChangeState(PlayerIdleState.StateType);
    }
    public override void Dodge()
    {
        ChangeState(PlayerDodgingState.StateType);
    }
    public override void InterruptState()
    {
        _context.shadowSpikeSkill.StopCastingSpikes();
        _context.shadowSpikeSkill.SetFullShadow(false);
    }
}