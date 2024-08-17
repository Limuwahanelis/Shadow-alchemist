using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PITShadowMoveState : PlayerInputTutorialState
{
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if(_context.shadowModifier==PlayerInputHandler.ShadowControlInputs.CONTROL) ChangeState(typeof(PITShadowControlState));
    }
}
