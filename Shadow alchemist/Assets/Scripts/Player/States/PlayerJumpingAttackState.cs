using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingAttackState : PlayerAttackState
{
    private float _jumpAttackTime;
    private bool _performAirAttack;
    private bool _performSlamAttack;
    public static Type StateType { get => typeof(PlayerJumpingAttackState); }
    public PlayerJumpingAttackState(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        //AttackCheck(PlayerCombat.AttackType.JUMPING);
        //if(_time > _jumpAttackTime)
        //{
        //    PerformInputCommand();
        //    if (_performAirAttack) ChangeState(PlayerInAirAttackingState.StateType);
        //    else if(_performSlamAttack) ChangeState(PlayerAirSlammingState.StateType);
        //    else
        //    {
        //        _context.playerMovement.SetRB(true);
        //        ChangeState(PlayerInAirState.StateType);
        //    }
        //}
    }
    public override void Attack(PlayerCombat.AttackModifiers modifier = PlayerCombat.AttackModifiers.NONE)
    {
        if(modifier == PlayerCombat.AttackModifiers.DOWN_ARROW) _performSlamAttack = true;
        else _performAirAttack = true;
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.animationManager.PlayAnimation("Jump Attack");
        _animSpeed = _context.animationManager.GetAnimationSpeed("Jump Attack");
        _jumpAttackTime = _context.animationManager.GetAnimationLength("Jump Attack")/_animSpeed;
        _currentAttack = _context.combat.JumpAttack;
        _attackDamageStartWindow = _currentAttack.AttackDamageWindowStart;
        _attackDamageEndWindow = _currentAttack.AttackDamageWindowEnd;
        _context.playerMovement.StopPlayer();
        _context.playerMovement.SetRB(false);

        _time = 0;
    }
    public override void UndoComand()
    {
        _performSlamAttack = false;
        _performAirAttack = false;
    }
    public override void InterruptState()
    {
        _performAirAttack = false;
        _performSlamAttack = false;
        _context.playerMovement.SetRB(true);
        if( _attackCor != null ) _context.coroutineHolder.StopCoroutine(_attackCor);
        _attackCor = null;
    }
}