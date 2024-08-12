using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;

public class PlayerInputTutorialContext
{
    public PlayerController playerController;
    public ShadowControlInputs shadowModifier;
    public Vector2 direction;
    public float horizontalModifier;
    public bool isDownArrowPressed;
}
