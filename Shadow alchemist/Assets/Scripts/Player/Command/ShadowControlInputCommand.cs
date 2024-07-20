using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowControlInputCommand : InputCommand
{
    private PlayerInputHandler.ShadowControlInputs _controlInput;
    public ShadowControlInputCommand(PlayerState playerState, PlayerInputHandler.ShadowControlInputs controlInput) : base(playerState)
    {
        _controlInput = controlInput;
    }

    public override void Execute()
    {
        _playerState.ControlShadow(_controlInput);
    }

    public override void Undo()
    {
        _playerState.UndoComand();
    }
}
