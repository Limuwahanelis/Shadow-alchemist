using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public static Type StateType { get => typeof(PlayerDeadState); }
    public PlayerDeadState(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        //PerformInputCommand();
        if(_context.checks.IsOnGround) _context.playerMovement.StopPlayer();
    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        _context.WaitFrameAndPerformFunction(() => { _context.animationManager.PlayAnimation("Dead"); });
        
    }

    public override void InterruptState()
    {
     
    }
}