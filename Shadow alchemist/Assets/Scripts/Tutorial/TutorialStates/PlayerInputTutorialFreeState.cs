using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInputTutorialFreeState : PlayerInputTutorialState
{
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if (_context.shadowModifier == ShadowControlInputs.ENTER)
        {
            if (_shadowsInteractions.Shadow != null)
            {
                ChangeState(typeof(PlayerInputTutorialShadowControlState));
                if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStep.ENTER_SHADOW_CONTRL_MODE)
                {
                    CompleteTutorialStep();
                }



            }
        }
    }


}
