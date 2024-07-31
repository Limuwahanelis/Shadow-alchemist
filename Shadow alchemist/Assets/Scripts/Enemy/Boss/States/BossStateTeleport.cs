using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateTeleport : EnemyState
{
    public static Type StateType { get => typeof(BossStateTeleport); }
    private BossContext _context;
    private float _teleportAnimationTime;
    private bool _isDisappearing = true;
    private bool _hasteleported = false;
    private float _time = 0;

    public BossStateTeleport(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        _time += Time.deltaTime;
        if (_isDisappearing)
        {
            if (_time > _teleportAnimationTime)
            {
                _isDisappearing = false;
                _time = 0;
            }
        }
        else
        {
            if (_hasteleported)
            {
                if (_time > _teleportAnimationTime)
                {
                    _context.movement.SetCollider(true);
                    _context.movement.SetRb(RigidbodyType2D.Dynamic);
                    switch (_context.currentPhase)
                    {
                        case BossController.BossPhase.MISSILES_TOP: ChangeState(BossStateCastTopMissiles.StateType);  break;
                        case BossController.BossPhase.NORMAL_ATTACK:ChangeState(BossStateAttacking.StateType); break;
                        case BossController.BossPhase.MISSLES_SIDES: ChangeState(BossStateCastSidesMissiles.StateType); break;
                        case BossController.BossPhase.CHARGE:ChangeState(BossStateCharge.StateType); break;
                    }
                }
            }
            else
            {
                if (_time > _teleportAnimationTime + 1f)
                {
                    _context.enemyTransform.position = _context.teleportPoints[_context.indexOfTeleportPos].position;
                    if (_context.currentPhase == BossController.BossPhase.NORMAL_ATTACK)
                    {
                        if (_context.indexOfTeleportPos == 3)
                        {
                            if (_context.movement.FlipSide == GlobalEnums.HorizontalDirections.LEFT)  _context.movement.FlipEnemy();
                        }
                        else if(_context.indexOfTeleportPos == 4)
                        {
                            if (_context.movement.FlipSide == GlobalEnums.HorizontalDirections.RIGHT) _context.movement.FlipEnemy();
                        }

                    }
                    else if(_context.currentPhase==BossController.BossPhase.CHARGE)
                    {
                        if (_context.indexOfTeleportPos == 0)
                        {
                            if (_context.movement.FlipSide == GlobalEnums.HorizontalDirections.LEFT) _context.movement.FlipEnemy();
                        }
                        else if (_context.indexOfTeleportPos == 2)
                        {
                            if (_context.movement.FlipSide == GlobalEnums.HorizontalDirections.RIGHT) _context.movement.FlipEnemy();
                        }
                    }
                    
                    _context.animMan.PlayAnimation("teleport show");
                    _hasteleported = true;
                    _time = 0;
                }
            }
        }
    }

    public override void SetUpState(EnemyContext context)
    {
        base.SetUpState(context);
        _context = (BossContext)context;
        _context.animMan.PlayAnimation("teleport hide");
        _teleportAnimationTime = _context.animMan.GetAnimationLength("teleport hide");
        _context.movement.SetForTeleprt();
        _isDisappearing = true;
        _hasteleported = false;
        _time = 0;
    }

    public override void InterruptState()
    {
     
    }
}