using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.InputSystem;
using System;

public abstract class PlayerInputTutorialState
{
    public delegate PlayerInputTutorialState GetState(Type state);
    protected GetState _getType;

    protected PlayerInputTutorialContext _context;
    protected bool _isDownArrowPressed;
    protected bool _useCommands;
    protected PlayerInputStack _inputStack;
    public PlayerInputTutorialState(GetState getState,bool useCommands, PlayerInputStack inputStack)
    {
        _getType = getState;
        _useCommands = useCommands;
        _inputStack = inputStack;
    }
    // Update is called once per frame
    public virtual void Update() { }
    public virtual void OnMove(InputValue value){ }
    public virtual void OnJump(InputValue value){}
    public virtual void OnVertical(InputValue value){}
    public virtual void OnHorizontalModifier(InputValue value){}
    public virtual void OnAttack(InputValue value){}

    public virtual void OnDownArrowModifier(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        _context.isDownArrowPressed = value.Get<float>() == 1 ? true : false;
    }
    public virtual void OnControlShadow(InputValue value)
    {
    }
}
