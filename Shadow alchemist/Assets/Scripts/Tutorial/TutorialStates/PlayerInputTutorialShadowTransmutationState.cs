using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputHandler;

public class PlayerInputTutorialShadowTransmutationState : PlayerInputTutorialState
{
    private bool _isTransmutating;
    private bool _isInShadowControl=false;
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if(_context.shadowModifier==ShadowControlInputs.CONTROL)
        {
            if(_tutorialStep==PlayerInputHandlerTutorial.TutorialStep.ENTER_SHADOW_CONTROL_MODE_2) CompleteTutorialStep();
            ChangeState(typeof(PlayerInputTutorialShadowControlState));
        }
    }
    public override void OnAttack(InputValue value)
    {
        base.OnAttack(value);
        if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStep.ENTER_SHADOW_CONTROL_MODE_2) CompleteTutorialStep();
        ChangeState(typeof(PlayerInputTutorialShadowControlState));
    }
}
