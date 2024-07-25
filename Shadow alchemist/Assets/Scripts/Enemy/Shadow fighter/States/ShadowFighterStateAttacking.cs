using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterStateAttacking : EnemyState
{
    public static Type StateType { get => typeof(ShadowFighterStateAttacking); }
    private ShadowFighterContext _context;

    protected float _attackDamageStartWindow;
    protected float _attackDamageEndWindow;
    protected float _animSpeed;
    protected bool _isDealingDmg;
    protected bool _checkForDmg = true;
    protected float _time;
    protected ComboAttack _currentAttack;
    protected Coroutine _attackCor;

    private int _comboCounter = 1;
    private bool _nextAttack;
    private int _maxCombo = 3;
    private float[] _attacksAnimLengths = new float[3];
    public ShadowFighterStateAttacking(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        //if (Vector2.Distance(_context.enemyTransform.position, _context.playerTransform.position) > _context.maxPlayerRange)
        //{
        //    ChangeState(ShadowFighterStateChasePlayer.StateType);
        //    _nextAttack = false;
        //}
        _time += Time.deltaTime;

        switch(_comboCounter)
        {
            case 1: AttackCheck(ShadowFighterCombat.AttackType.Fist1Attack1);break;
            case 2: AttackCheck(ShadowFighterCombat.AttackType.Fist1Attack2); break;
            case 3: AttackCheck(ShadowFighterCombat.AttackType.Fist2Attack1);break;
        }

        if (_context.movement.FlipSide == GlobalEnums.HorizontalDirections.RIGHT)
        {
            if (_context.playerTransform.position.x < _context.enemyTransform.position.x) _nextAttack = false;
            else if (_context.playerTransform.position.x < _context.enemyTransform.position.x + _context.maxPlayerRange) _nextAttack = true;
            else _nextAttack = false;
        }
        else
        {
            if (_context.playerTransform.position.x > _context.enemyTransform.position.x) _nextAttack = false;
            else if (_context.playerTransform.position.x > _context.enemyTransform.position.x - _context.maxPlayerRange) _nextAttack = true;
            else _nextAttack = false;
        }



        if (_nextAttack)
        {

            if (_time >= _attacksAnimLengths[_comboCounter - 1] + _context.combat.AttacksDelays[_comboCounter - 1])
            {
                _time = 0;
                _comboCounter++;
                if (_comboCounter > _maxCombo)
                {
                    _comboCounter = 1;
                    ChangeState(ShadowFighterStateIdle.StateType);
                    return;
                }
                _context.animMan.PlayAnimation($"Attack {_comboCounter}");
                _currentAttack = _context.combat.ShadowFighterCombos.comboList[_comboCounter - 1];
                _animSpeed = _context.animMan.GetAnimationSpeed("Attack " + _comboCounter, "Base Layer");
                _nextAttack = false;
                _isDealingDmg = false;
                _checkForDmg = true;
            }
        }
        else if (_time >= _attacksAnimLengths[_comboCounter - 1] + _context.combat.AttacksDelays[_comboCounter - 1])
        {
            _comboCounter++;
            if (_comboCounter > _maxCombo)
            {
                _comboCounter = 1;
            }
            ChangeState(ShadowFighterStateIdle.StateType);
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (ShadowFighterContext)context;

        _isDealingDmg = false;
        _time = 0;
        _nextAttack = false;
        _attackCor = null;
        _checkForDmg = true;
        _context.animMan.PlayAnimation("Attack " + _comboCounter);
        _animSpeed = _context.animMan.GetAnimationSpeed("Attack " + _comboCounter, "Base Layer");
        _currentAttack = _context.combat.ShadowFighterCombos.comboList[_comboCounter - 1];
        for (int i = 0; i < _maxCombo; i++)
        {
            _attacksAnimLengths[i] = _context.animMan.GetAnimationLength("Attack " + (i+1)) / _context.animMan.GetAnimationSpeed("Attack " + (i+1));
        }
        //if(_comboCounter)
        //_currentAttack = _context.combat.SkeletonCombos.comboList[_comboCounter - 1];
    }
    public virtual void AttackCheck(ShadowFighterCombat.AttackType attackType)
    {
        if (_checkForDmg)
        {
            if (!_isDealingDmg)
            {
                if (_time > _context.combat.ShadowFighterCombos.comboList[((int)attackType)].AttackDamageWindowStart)
                {
                    _attackCor = _context.coroutineHolder.StartCoroutine(_context.combat.AttackCor(attackType));
                    _isDealingDmg = true;
                    _checkForDmg = false;
                }
            }
            else
            {
                if (_time > _context.combat.ShadowFighterCombos.comboList[((int)attackType)].AttackDamageWindowEnd)
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