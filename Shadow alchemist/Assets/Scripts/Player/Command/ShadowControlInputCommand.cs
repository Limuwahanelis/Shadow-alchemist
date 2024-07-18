using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowControlInputCommand : InputCommand
{
    public ShadowControlInputCommand(PlayerState playerState) : base(playerState)
    {
    }

    public override void Execute()
    {
        _playerState.ControlShadow();
    }

    public override void Undo()
    {
        _playerState.UndoComand();
    }
}
