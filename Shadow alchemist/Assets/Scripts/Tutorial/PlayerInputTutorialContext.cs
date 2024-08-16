using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static PlayerInputHandler;

public class PlayerInputTutorialContext
{
    public PlayerController playerController;
    public ShadowControlInputs shadowModifier;
    public Vector2 direction;
    public Action<PlayerInputTutorialState> ChangeTutorialState;
    public float horizontalModifier;
    public bool isDownArrowPressed;
    public UnityEvent OnTutorialStepCompleted;
    public Action UpdateTutorialStep;
}
