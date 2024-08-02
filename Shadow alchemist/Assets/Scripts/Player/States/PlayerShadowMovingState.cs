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
        _context.shadowControl.OnControllableShadowLeft += GoBackToIdle;
    }
    public override void Update()
    {
        PerformInputCommand();
        
    }
    public override void Move(Vector2 direction)
    {
        _context.shadowControl.Shadow.MoveShadow(2f, direction);
    }
    public override void Attack(PlayerCombat.AttackModifiers modifier)
    {
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void Jump()
    {
        _context.shadowControl.Shadow.RevertMove();
    }
    private void GoBackToIdle()
    {
        _context.shadowControlModeSelectionUI.SetVisiblity(false);
        ChangeState(PlayerIdleState.StateType);
    }
    public override void InterruptState()
    {
        _context.shadowControl.OnControllableShadowLeft -= GoBackToIdle;
    }
}