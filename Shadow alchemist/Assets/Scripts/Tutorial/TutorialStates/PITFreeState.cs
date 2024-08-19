using static PlayerInputHandler;
using UnityEngine.InputSystem;
public class PITFreeState : PlayerInputTutorialState
{
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if (_context.shadowModifier == ShadowControlInputs.ENTER)
        {
            if (_shadowsInteractions.Shadow != null)
            {
                ChangeState(typeof(PITShadowControlState));
                if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStepEn.ENTER_SHADOW_CONTRL_MODE)
                {
                    CompleteTutorialStep();
                }



            }
        }
    }


}
