using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BossStateCharge : EnemyState
{
    public static Type StateType { get => typeof(BossStateCharge); }
    private BossContext _context;
    private bool _startedCheckForDMG;
    protected Coroutine _attackCor;
    public BossStateCharge(GetState function) : base(function)
    {
    }

    public override void Update()
    {

        //if(_context.chargeInfo.ShouldStartCharge)
        //{
        //    if (_context.indexOfTeleportPos == 0) _context.movement.Move(Vector2.right);
        //    else if( _context.indexOfTeleportPos == 2) _context.movement.Move(Vector2.left);
        //    if (!_startedCheckForDMG)
        //    {
        //        _startedCheckForDMG = true;
        //        _attackCor = _context.coroutineHolder.StartCoroutine(_context.combat.AttackCor(BossCombat.AttackType.CHARGE));
        //    }
        //}
        //if(_context.indexOfTeleportPos)
    }
    public override void FixedUpdate()
    {
        if (_context.chargeInfo.ShouldStartCharge)
        {
            if (_context.indexOfTeleportPos == 0) _context.movement.Move(Vector2.right);
            else if (_context.indexOfTeleportPos == 2) _context.movement.Move(Vector2.left);
            if (!_startedCheckForDMG)
            {
                _startedCheckForDMG = true;
                _attackCor = _context.coroutineHolder.StartCoroutine(_context.combat.AttackCor(BossCombat.AttackType.CHARGE));
            }
        }
        if (_context.indexOfTeleportPos == 2)
        {
            if (_context.enemyTransform.position.x < _context.leftChargeStop.position.x)
            {
                _context.chargeInfo.ResetCharge();
                _context.animMan.PlayAnimation("Charge stop");
                _context.movement.Stop();
            }
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.animMan.PlayAnimation("Charge");
        _startedCheckForDMG = false;
        // for tests
        _context.indexOfTeleportPos = 2;
    }

    public override void Hit(DamageInfo damageInfo)
    {
        Logger.Log("Change to wall hit state");
    }
    public override void InterruptState()
    {
     
    }
}