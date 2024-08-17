using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PITShadowPlacementState : PlayerInputTutorialState
{
    private bool _isSelectingPlacableShadow = true;
    private bool _isPlacingShadow = false;
    public override void OnControlShadow(InputValue value)
    {
        base.OnControlShadow(value);
        if(_context.shadowModifier==PlayerInputHandler.ShadowControlInputs.CONTROL)
        {
            ChangeState(typeof(PITShadowControlState));
            _isSelectingPlacableShadow = true;
            return;
        }
        if (_shadowsInteractions.FullShadow)
        {
            ChangeState(typeof(PITFreeState));
            _isSelectingPlacableShadow = true;
            return;
        }
    }
    public override void OnAttack(InputValue value)
    {
        base.OnAttack(value);
        if (_isSelectingPlacableShadow)
        {
            if (_shadowsInteractions.FullShadow)
            {
                _isPlacingShadow = true;
            }
            else if (_shadowsInteractions.Shadow.TakenShadowBarValueFromTransmutation >= _placableShadowSelection.CurrentlyHighlihtedShadow.ShadowBarCost)
            {
                // select which shadow to spawn, spawn it but not place it yet
                _isSelectingPlacableShadow = false;
                _isPlacingShadow = true;
                if(_tutorialStep==PlayerInputHandlerTutorial.TutorialStep.PLACE_SHADOW) CompleteTutorialStep();
            }

        }
        else if (_isPlacingShadow)
        {
            if (_shadowsInteractions.ShadowToPlace.CanBePlaced)
            {
                _isPlacingShadow = false;
                _isSelectingPlacableShadow = true;
                if (_tutorialStep == PlayerInputHandlerTutorial.TutorialStep.PLACE_SHADOW) CompleteTutorialStep();
                // place shadow
            }
        }
    }
    public override void OnJump(InputValue value)
    {
        base.OnJump(value);
        if (_isSelectingPlacableShadow)
        {
            if (_shadowsInteractions.FullShadow) return;

        }
        else
        {
            _isPlacingShadow= false;
        }
    }
}
