using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PITShadowControlState : PlayerInputTutorialState
{
    [SerializeField] TutorialStep aa;
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if(_context.shadowModifier==PlayerInputHandler.ShadowControlInputs.TRANSMUTATE)
        {
            ChangeState(typeof(PITShadowTransmutationState));
            if(_tutorialStep==PlayerInputHandlerTutorial.TutorialStepEn.ENTER_SHADOW_TRANSMUTATION) CompleteTutorialStep();
        }
    }
    public override void OnAttack(InputValue value)
    {
        base.OnAttack(value);
        if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStepEn.ENTER_SHADOW_PLACEMENT)CompleteTutorialStep();
        ChangeState(typeof(PITShadowPlacementState));
    }
    public override void OnJump(InputValue value)
    {
        base.OnJump(value);
        if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStepEn.MOVE_SHADOW) CompleteTutorialStep();
        ChangeState(typeof(PITShadowMoveState));
    }
}
