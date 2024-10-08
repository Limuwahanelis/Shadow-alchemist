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
        //if(_context.shadowControl.Shadow==null)
        //{
        //    ChangeState(PlayerIdleState.StateType);
        //    return;
        //}
        PerformInputCommand();
    }
    public override void Move(Vector2 direction)
    {
        //if (direction == Vector2.zero) return;
        if (_context.shadowControl.ShadowBar.CurrentValue >= 5) return;
        _context.shadowControl.Shadow.Transmutate(direction);
    }
    public override void Attack(PlayerCombat.AttackModifiers modifier = PlayerCombat.AttackModifiers.NONE)
    {
        _context.shadowControl.Shadow.RevertNonSegmentShadowbar();
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        _context.shadowControl.Shadow.RevertNonSegmentShadowbar();
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void Jump()
    {
        _context.shadowControl.Shadow.RevertTransmutation();
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.shadowControl.OnControllableShadowLeft += GoBackToIdle;
    }
    private void GoBackToIdle()
    {
        _context.shadowControl.Shadow.RevertNonSegmentShadowbar();
        _context.shadowControlModeSelectionUI.SetVisiblity(false);
        ChangeState(PlayerIdleState.StateType);
    }
    public override void InterruptState()
    {
        _context.shadowControl.OnControllableShadowLeft -= GoBackToIdle;
    }
}