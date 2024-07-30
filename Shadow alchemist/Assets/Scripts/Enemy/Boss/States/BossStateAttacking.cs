using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateAttacking : EnemyState
{
    public static Type StateType { get => typeof(BossStateAttacking); }
    private BossContext _context;
    private float _time;
    private float _attackAnimLength;

    protected bool _isDealingDmg;
    protected bool _checkForDmg = true;
    protected ComboAttack _currentAttack;
    protected Coroutine _attackCor;
    public BossStateAttacking(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        AttackCheck(BossCombat.AttackType.NORMAL_ATTACK);
        if (_time >= _attackAnimLength)
        {
            _context.currentPhase = BossController.BossPhase.MISSLES_SIDES;
            _context.indexOfTeleportPos = 1;
            ChangeState(BossStateTeleport.StateType);
            return;
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.animMan.PlayAnimation("Attack");
        _attackAnimLength= _context.animMan.GetAnimationLength("Attack");
        _time = 0;
    }
    public virtual void AttackCheck(BossCombat.AttackType attackType)
    {
        if (_checkForDmg)
        {
            if (!_isDealingDmg)
            {
                if (_time >= _context.combat.ShadowFighterCombos.comboList[((int)attackType)].AttackDamageWindowStart)
                {
                    _attackCor = _context.coroutineHolder.StartCoroutine(_context.combat.AttackCor(attackType));
                    _isDealingDmg = true;
                    _checkForDmg = false;
                }
            }
            else
            {
                if (_time >= _context.combat.ShadowFighterCombos.comboList[((int)attackType)].AttackDamageWindowEnd)
                {
                    _context.coroutineHolder.StopCoroutine(_attackCor);
                    _attackCor = null;
                    _checkForDmg = false;
                }
            }
        }
    }
    public override void InterruptState()
    {
     
    }
}