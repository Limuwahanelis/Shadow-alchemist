using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    public static Type StateType { get => typeof(PlayerInAirState); }
    private bool _isFalling;
    private bool _jumpOnLanding;
    public PlayerInAirState(GetState function) : base(function)
    {
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.playerMovement.SetRBMaterial(PlayerMovement.PhysicMaterialType.NO_FRICTION);
    }
    public override void Update()
    {
        PerformInputCommand();
        if(_context.playerMovement.IsPlayerFalling)
        {
            if (!_isFalling)
            {
                _context.animationManager.PlayAnimation("Fall");
                _isFalling = true;
            }
        }
        if(_context.checks.IsOnGround && math.abs( _context.playerMovement.PlayerRB.velocity.y) < 0.0004) 
        {
            _context.playerMovement.SetRBMaterial(PlayerMovement.PhysicMaterialType.NORMAL);
            if (_stateTypeToChangeFromInputCommand != null)
            {
                ChangeState(_stateTypeToChangeFromInputCommand);
                _stateTypeToChangeFromInputCommand = null;
            }
            else ChangeState(PlayerIdleState.StateType);
        }
        
    }
    public override void Attack(PlayerCombat.AttackModifiers modifier = PlayerCombat.AttackModifiers.NONE)
    {
        _stateTypeToChangeFromInputCommand = PlayerAttackingState.StateType;
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        switch (controlInput)
        {
            case PlayerInputHandler.ShadowControlInputs.ENTER:
                {
                    if (_context.shadowControl.Shadow != null) _stateTypeToChangeFromInputCommand = PlayerShadowControlState.StateType; break;
                }
            case PlayerInputHandler.ShadowControlInputs.SHADOW_SPIKE:
                {
                    if (_context.shadowControl.Shadow != null) _stateTypeToChangeFromInputCommand = PlayerCastingShadowSpikesState.StateType; break;
                }

        }

    }
    public override void Jump()
    {
        _stateTypeToChangeFromInputCommand = PlayerJumpingState.StateType;
    }
    public override void Move(Vector2 direction)
    {
        _context.playerMovement.Move(direction.x,true);
    }
    //public override void Attack(PlayerCombat.AttackModifiers modifier)
    //{
    //    if (modifier == PlayerCombat.AttackModifiers.DOWN_ARROW)
    //    {
    //        ChangeState(PlayerAirSlammingState.StateType);
    //        return;
    //    }
    //    if(_context.canPerformAirCombo) ChangeState(PlayerInAirAttackingState.StateType);
    //}
    public override void Push()
    {
        base.Push();
    }
    public override void UndoComand()
    {
        _jumpOnLanding = false;
        _stateTypeToChangeFromInputCommand = null;
    }
    public override void InterruptState()
    {
        _isFalling = false;
        _jumpOnLanding = false;
    }
}