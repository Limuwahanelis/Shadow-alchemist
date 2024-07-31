using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public static Type StateType { get => typeof(PlayerWalkingState); }
    public PlayerWalkingState(GetState function) : base(function)
    {
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.animationManager.PlayAnimation("Walk");
        _context.canPerformAirCombo = true;
    }
    public override void Update()
    {
        PerformInputCommand();
        if (!_context.checks.IsOnGround) ChangeState(PlayerInAirState.StateType);
    }
    public override void Move(Vector2 direction)
    {
        if (direction.x == 0)
        {
            ChangeState(PlayerIdleState.StateType);
        }
        _context.playerMovement.Move(direction.x);
    }
    public override void Jump()
    {
        ChangeState(PlayerJumpingState.StateType);
    }
    public override void Dodge()
    {
        ChangeState(PlayerDodgingState.StateType);
    }
    public override void Attack(PlayerCombat.AttackModifiers modifier)
    {
        switch (modifier)
        {
            case PlayerCombat.AttackModifiers.NONE: ChangeState(PlayerAttackingState.StateType); break;
            //case PlayerCombat.AttackModifiers.UP_ARROW: ChangeState(PlayerJumpingAttackState.StateType); break;
        }

    }

    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        if(controlInput==PlayerInputHandler.ShadowControlInputs.SHADOW_SPIKE)
        {
            if(_context.shadowControl.Shadow || _context.shadowControl.FullShadow)
            {
                ChangeState(PlayerCastingShadowSpikesState.StateType);
            }
        }
    }
    public override void InterruptState()
    {
     
    }
}