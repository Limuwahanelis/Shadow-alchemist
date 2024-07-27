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
        PerformInputCommand();
    }
    public override void Move(Vector2 direction)
    {

    }
    public override void Attack(PlayerCombat.AttackModifiers modifier)
    {
        _context.shadowControlModeSelectionUI.SetVisiblity(false);
        ChangeState(PlayerShadowPlacingState.StateType);
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        if (controlInput == PlayerInputHandler.ShadowControlInputs.TRANSMUTATE)
        {
            _context.shadowControlModeSelectionUI.SelectShadowTransmutation();
            ChangeState(PlayerShadowTransmutationState.StateType);
        }
        else
        {
            _context.shadowControlModeSelectionUI.Deselect();
            _context.shadowControlModeSelectionUI.SetVisiblity(false);
            ChangeState(PlayerIdleState.StateType);// _isMovingShadow = !_isMovingShadow;
        }

    }
    public override void Jump()
    {
        _context.shadowControlModeSelectionUI.SelectMoveShadow();
        ChangeState(PlayerShadowMovingState.StateType);

    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.shadowControlModeSelectionUI.Deselect();
        _context.shadowControlModeSelectionUI.SetShadowType(_context.shadowControl.Shadow.IsHorizontal);
        _context.shadowControlModeSelectionUI.SetVisiblity(true);
        _context.animationManager.PlayAnimation("Shadow cast");
        //_isTransmutatingShadow = false;
        //if(_context.shadowControl.Shadow==null) ChangeState(PlayerIdleState.StateType);
    }

    public override void InterruptState()
    {
    }
}