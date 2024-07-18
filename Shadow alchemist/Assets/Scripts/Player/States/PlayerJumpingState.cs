using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerState
{
    public static Type StateType { get => typeof(PlayerJumpingState); }

    private Coroutine _jumpCor;
    public PlayerJumpingState(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        PerformInputCommand();
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        //_context.animationManager.PlayAnimation("Jump");
        _context.playerMovement.StopPlayer();
       //_jumpCor = _context.WaitAndPerformFunction(_context.animationManager.GetAnimationLength("Jump"), () => {
           _context.playerMovement.Jump();
           ChangeState(PlayerInAirState.StateType);
       //});
    }

    public override void InterruptState()
    {
        if(_jumpCor != null) _context.coroutineHolder.StopCoroutine(_jumpCor);
    }

    public override void UndoComand()
    {
      
    }
}