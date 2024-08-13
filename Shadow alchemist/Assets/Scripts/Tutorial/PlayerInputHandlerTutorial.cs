using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using System.Linq;

public class PlayerInputHandlerTutorial : MonoBehaviour
{
    public enum TutorialStep
    {
        ENTER_SHADOW_CONTRL_MODE,ENTER_SHADOW_TRANSMUTATION,ENTER_SHADOW_PLACEMENT,PLACE_SHADOW
    }

    public UnityEvent OnShadowControlModeFirstEntered;
    public UnityEvent OnShadowTransmutationModeFirstEntered;
    public UnityEvent OnShadowPlacementFirstEntered;
    //public UnityEvent OnFirstShadowPlaced;

    [SerializeField] PlayerController _player;
    [SerializeField] InputActionAsset _controls;
    [SerializeField] PlayerInputStack _inputStack;
    [SerializeField] bool _useCommands;
    [SerializeField] bool _printState;

    private Dictionary<Type, PlayerInputTutorialState> _tutorialStates = new Dictionary<Type, PlayerInputTutorialState>();
    private PlayerInputTutorialState _currentTutorialState;
    PlayerInputTutorialContext _context;
    TutorialStep _currentStep;

    void Start()
    {
        List<Type> states = AppDomain.CurrentDomain.GetAssemblies().SelectMany(domainAssembly => domainAssembly.GetTypes())
    .Where(type => typeof(PlayerInputTutorialState).IsAssignableFrom(type) && !type.IsAbstract).ToArray().ToList();

        _player = GetComponent<PlayerController>();

        _context = new PlayerInputTutorialContext()
        {
            playerController = _player,
            ChangeTutorialState = ChangeState,
            OnStateExit = OnShadowControlModeFirstEntered,
        };

        PlayerInputTutorialState.GetState getState = GetState;
        PlayerInputTutorialState.SetUp(_context, getState, _useCommands, _inputStack);
        PlayerInputTutorialState.SetTutorialStep(TutorialStep.ENTER_SHADOW_CONTRL_MODE);
        foreach (Type state in states)
        {
            _tutorialStates.Add(state, (PlayerInputTutorialState)Activator.CreateInstance(state));
        }
        PlayerInputTutorialState newState = GetState(typeof(PlayerInputTutorialFreeState));
        _currentTutorialState = newState;
    }
    public void ChangeState(PlayerInputTutorialState newState)
    {
        if (_printState) Logger.Log(newState.GetType());
        _currentTutorialState.ExitState();
        _currentTutorialState = newState;
    }
    public PlayerInputTutorialState GetState(Type state)
    {
        return _tutorialStates[state];
    }
    // Update is called once per frame
    void Update()
    {
        _currentTutorialState.Update();
    }
    public void ChangeEventOnExit()
    {
        _currentStep++;
        switch (_currentStep)
        {
            case TutorialStep.ENTER_SHADOW_TRANSMUTATION:_context.OnStateExit = OnShadowTransmutationModeFirstEntered;break;
            case TutorialStep.ENTER_SHADOW_PLACEMENT: _context.OnStateExit = OnShadowPlacementFirstEntered;break;
        }
        PlayerInputTutorialState.SetTutorialStep(_currentStep);
    }
    #region Inputs
    private void OnMove(InputValue value)
    {
        _currentTutorialState.OnMove(value);
    }
    void OnJump(InputValue value)
    {
        _currentTutorialState.OnJump(value);
    }
    void OnVertical(InputValue value)
    {
        _currentTutorialState.OnVertical(value);
    }
    void OnHorizontalModifier(InputValue value)
    {
        _currentTutorialState.OnHorizontalModifier(value);
    }
    private void OnAttack(InputValue value)
    {
        _currentTutorialState.OnAttack(value);
    }
    private void OnControlShadow(InputValue value)
    {
        _currentTutorialState.OnControlShadow(value);
    }
    private void OnInteract(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
    }
    private void OnWarp(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        _player.CurrentPlayerState.Dodge();
    }
    #endregion
}
