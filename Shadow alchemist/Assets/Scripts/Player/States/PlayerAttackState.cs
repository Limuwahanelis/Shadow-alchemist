using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAttackState : PlayerState
{
    protected float _attackDamageStartWindow;
    protected float _attackDamageEndWindow;
    protected float _animSpeed;
    protected bool _isDealingDmg;
    protected bool _checkForDmg = true;
    protected float _time;
    protected ComboAttack _currentAttack;
    protected Coroutine _attackCor;
    protected PlayerAttackState(GetState function) : base(function)
    {
    }
    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _attackCor = null;
        _checkForDmg = true;
        _isDealingDmg = false;
        _time = 0;
    }
    public virtual void AttackCheck(PlayerCombat.AttackType attackType)
    {
        if (_checkForDmg)
        {
            if (!_isDealingDmg)
            {
                if (_time > _attackDamageStartWindow)
                {
                    _attackCor = _context.coroutineHolder.StartCoroutine(_context.combat.AttackCor(attackType));
                    _isDealingDmg = true;
                }
            }
            else
            {
                if (_time > _attackDamageEndWindow)
                {
                    _context.coroutineHolder.StopCoroutine(_attackCor);
                    _attackCor = null;
                    _checkForDmg = false;
                }
            }
        }
    }
}