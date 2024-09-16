using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchedPushedState : PlayerState
{
    public static Type StateType { get => typeof(PlayerCrouchedPushedState); }
    public PlayerCrouchedPushedState(GetState function) : base(function)
    {
    }

    public override void Update()
    {
    }
    public override void Move(Vector2 direction)
    {
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.animationManager.PlayAnimation("Crouch idle");
        _context.animationManager.SetAnimator(false);
        _context.collisions.SetNormalColliders(false);
        _context.collisions.SetCrouchColliiers(true);
        _context.combat.ChangeSpriteToCoruchPushed();
        _context.shadowControlModeSelectionUI.SetVisiblity(false);
        _context.placableShadowSelection.SetSelectionVisibility(false);
        _context.WaitAndPerformFunction(0.2f,() =>
        {
            _context.animationManager.SetAnimator(true);
            ChangeState(PlayerCrouchingIdleState.StateType);
        });
    }
    public override void Push()
    {

    }
    public override void InterruptState()
    {
        _context.animationManager.SetAnimator(true);
    }
}