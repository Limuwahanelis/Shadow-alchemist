using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputTutorialShadowControlState : PlayerInputTutorialState
{
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if(_context.shadowModifier==PlayerInputHandler.ShadowControlInputs.TRANSMUTATE)
        {
            ChangeState(typeof(PlayerInputTutorialShadowTransmutationState));
            if(_tutorialStep==PlayerInputHandlerTutorial.TutorialStep.ENTER_SHADOW_TRANSMUTATION) CompleteTutorialStep();
        }
    }
    public override void OnAttack(InputValue value)
    {
        base.OnAttack(value);
        if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStep.ENTER_SHADOW_PLACEMENT)CompleteTutorialStep();
        ChangeState(typeof(PlayerInputTutorialEnterShadowPlacementState));
    }
}
