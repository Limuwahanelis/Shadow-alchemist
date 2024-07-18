using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public static Type StateType { get => typeof(PlayerIdleState); }
    public PlayerIdleState(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        PerformInputCommand();
    }
    public override void Move(Vector2 direction)
    {
        if (Math.Abs(direction.x) > 0) ChangeState(PlayerWalkingState.StateType);
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        //_context.animationManager.PlayAnimation("Idle");
        _context.canPerformAirCombo = true;
    }
    public override void Jump()
    {
        ChangeState(PlayerJumpingState.StateType);
    }
    public override void Dodge()
    {
        ChangeState(PlayerDodgingState.StateType);
    }
    //public override void Attack(PlayerCombat.AttackModifiers modifier)
    //{
    //    switch (modifier)
    //    {
    //        case PlayerCombat.AttackModifiers.NONE: ChangeState(PlayerAttackingState.StateType); break;
    //        case PlayerCombat.AttackModifiers.UP_ARROW: ChangeState(PlayerJumpingAttackState.StateType); break;
    //    }
    //}
    public override void InterruptState()
    {
        
    }
}