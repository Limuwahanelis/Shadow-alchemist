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
    protected static GetState _getType;
    protected static PlayerInputHandlerTutorial.TutorialStepEn _tutorialStep;

    protected static PlayerInputTutorialContext _context;
    protected static bool _isDownArrowPressed;
    protected static bool _useCommands;
    protected static PlayerInputStack _inputStack;
    protected static PlayerShadowsInteractions _shadowsInteractions;
    protected static PlacableShadowSelection _placableShadowSelection;
    protected static bool _fireEvents;
    // Update is called once per frame
    public virtual void Update() 
    {
        if (_context.playerController.IsAlive)
        {

            if (!PauseSettings.IsGamePaused)
            {
                _context.playerController.CurrentPlayerState.Move(_context.direction);
                
            }
        }
    }
    public virtual void OnMove(InputValue value)
    {
        _context.direction = value.Get<Vector2>();
    }
    public virtual void OnJump(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        if (_useCommands) _inputStack.CurrentCommand = new JumpInputCommand(_context.playerController.CurrentPlayerState);
        else _context.playerController.CurrentPlayerState.Jump();
    }
    public virtual void OnVertical(InputValue value)
    {
        _context.direction = value.Get<Vector2>();
    }
    public virtual void OnAttack(InputValue value)
    {

        if (PauseSettings.IsGamePaused) return;
        if (_useCommands)
        {
            _inputStack.CurrentCommand = new AttackInputCommand(_context.playerController.CurrentPlayerState);
            if (_context.direction.y > 0) _inputStack.CurrentCommand = new AttackInputCommand(_context.playerController.CurrentPlayerState, PlayerCombat.AttackModifiers.UP_ARROW);
            if (_context.direction.y < 0) _inputStack.CurrentCommand = new AttackInputCommand(_context.playerController.CurrentPlayerState, PlayerCombat.AttackModifiers.DOWN_ARROW);
        }
        else
        {

            if (_context.direction.y == 0) _context.playerController.CurrentPlayerState.Attack();
            else if (_context.direction.y > 0) _context.playerController.CurrentPlayerState.Attack(PlayerCombat.AttackModifiers.UP_ARROW);
            else if (_context.direction.y < 0) _context.playerController.CurrentPlayerState.Attack(PlayerCombat.AttackModifiers.DOWN_ARROW);
        }
    }

    public virtual void OnDownArrowModifier(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        _context.isDownArrowPressed = value.Get<float>() == 1 ? true : false;
    }
    public virtual void OnControlShadow(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        _context.shadowModifier = (ShadowControlInputs)value.Get<float>();
        if (_useCommands) _inputStack.CurrentCommand = new ShadowControlInputCommand(_context.playerController.CurrentPlayerState, _context.shadowModifier);
        else _context.playerController.CurrentPlayerState.ControlShadow(_context.shadowModifier);
    }
    public virtual void CompleteTutorialStep()
    {
        if (!_fireEvents)
        {
            return;
        }
        //_context.OnTutorialStepCompleted?.Invoke();
        _context.UpdateTutorialStep();
    }
    public void ChangeState(Type newStateType)
    {
        PlayerInputTutorialState state = _getType(newStateType);
        _context.ChangeTutorialState(state);
    }
    public static void SetUp(PlayerInputTutorialContext context, GetState getState, bool useCommands, PlayerInputStack inputStack,PlayerShadowsInteractions shadowsInteractions,PlacableShadowSelection placableShadowSelection,bool fireEvents)
    {
        _context = context;
        _getType = getState;
        _useCommands = useCommands;
        _inputStack = inputStack;
        _shadowsInteractions = shadowsInteractions;
        _placableShadowSelection = placableShadowSelection;
        _fireEvents = fireEvents;
    }

    public static void SetTutorialStep(PlayerInputHandlerTutorial.TutorialStepEn tutorialStep)
    {
        _tutorialStep = tutorialStep;
    }
}
