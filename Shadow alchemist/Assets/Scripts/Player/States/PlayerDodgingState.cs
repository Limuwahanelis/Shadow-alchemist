using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgingState : PlayerState
{
    public static Type StateType { get => typeof(PlayerDodgingState); }
    private float _time;
    private float _dodgeTime;
    public PlayerDodgingState(GetState function) : base(function)
    {
        
    }

    public override void Update()
    {
        PerformInputCommand();
        _time += Time.deltaTime;
        if (_time >= _dodgeTime)
        {
            _context.playerMovement.StopPlayer();
                ChangeState(PlayerIdleState.StateType);
            _context.playerDodge.SetEnemyCollider(true);
        }
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _dodgeTime = _context.animationManager.GetAnimationLength("Dodge");///_context.animationManager.GetAnimationSpeed("Dodge");

#if UNITY_EDITOR
        _dodgeTime /= _context.animationManager.GetAnimationSpeedEditor("Dodge");
#else 
        _dodgeTime /= _context.animationManager.GetAnimationSpeed("Dodge");
#endif
        Logger.Log("doge start");
        _time = 0;
        Logger.Log(_dodgeTime);
        _context.animationManager.PlayAnimation("Dodge");
        _context.playerMovement.Dodge();
        _context.playerDodge.SetEnemyCollider(false);
    }

    public override void Dodge()
    {
        
    }

    public override void InterruptState()
    {
        
    }
}