using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;
using Unity.VisualScripting;
using System;
using System.Linq;

public class PlayerInputHandlerTutorial : MonoBehaviour
{
    public enum TutorialStep
    {
        ENTER_SHADOW_CONTRL_MODE,ENTER_SHADOW_TRANSMUTATION,ENTER_SHADOW_PLACEMENT,PLACE_SHADOW
    }


    public UnityEvent OnFirstShadowPlaced;

    [SerializeField] PlayerController _player;
    [SerializeField] InputActionAsset _controls;
    [SerializeField] PlayerInputStack _inputStack;
    [SerializeField] bool _useCommands;
    ShadowControlInputs shadowModifier;
    private TutorialStep _currentStep;
    private Vector2 _direction;
    private float _horizontalModifier;
    private int _numberOfAttackPressed = 0;
    private bool isDownArrowPressed;
    private bool _placedFirstShadow = false;


    private Dictionary<Type, PlayerInputTutorialState> _tutorialStates;
    private PlayerInputTutorialState _currentTutorialState;

    void Start()
    {
        List<Type> states = AppDomain.CurrentDomain.GetAssemblies().SelectMany(domainAssembly => domainAssembly.GetTypes())
    .Where(type => typeof(PlayerInputTutorialState).IsAssignableFrom(type) && !type.IsAbstract).ToArray().ToList();

        _player = GetComponent<PlayerController>();


        PlayerInputTutorialState.GetState getState = GetState;
        foreach (Type state in states)
        {
            _tutorialStates.Add(state, (PlayerInputTutorialState)Activator.CreateInstance(state, getState,_useCommands,_inputStack));
        }
        PlayerInputTutorialState newState = GetState(typeof(PlayerInputTutorialFreeState));
        _currentTutorialState = newState;
    }
    public PlayerInputTutorialState GetState(Type state)
    {
        return _tutorialStates[state];
    }
    // Update is called once per frame
    void Update()
    {
        if (_player.IsAlive)
        {

            if (!PauseSettings.IsGamePaused)
            {
                _player.CurrentPlayerState.Move(_direction);

            }
        }
    }
    private void OnMove(InputValue value)
    {
        _direction = value.Get<Vector2>();

    }
    void OnJump(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        if (_useCommands) _inputStack.CurrentCommand = new JumpInputCommand(_player.CurrentPlayerState);
        else _player.CurrentPlayerState.Jump();
        //if (direction * _player.mainBody.transform.localScale.x > 0 && isDownArrowPressed) _player.currentState.Slide();

    }
    void OnVertical(InputValue value)
    {
        _direction = value.Get<Vector2>();
        Logger.Log(_direction);
    }
    void OnHorizontalModifier(InputValue value)
    {
        _horizontalModifier = value.Get<float>();
    }
    private void OnAttack(InputValue value)
    {

        if (PauseSettings.IsGamePaused) return;
        if ((_player.CurrentPlayerState as PlayerShadowPlacingState) != null)
        {
            _numberOfAttackPressed++;
            if (_numberOfAttackPressed == 2)
            {
                if (!_placedFirstShadow)
                {
                    OnFirstShadowPlaced?.Invoke();
                }
            }
        }
        if (_useCommands)
        {
            _inputStack.CurrentCommand = new AttackInputCommand(_player.CurrentPlayerState);
            if (_direction.y > 0) _inputStack.CurrentCommand = new AttackInputCommand(_player.CurrentPlayerState, PlayerCombat.AttackModifiers.UP_ARROW);
            if (_direction.y < 0) _inputStack.CurrentCommand = new AttackInputCommand(_player.CurrentPlayerState, PlayerCombat.AttackModifiers.DOWN_ARROW);
        }
        else
        {

            if (_direction.y == 0) _player.CurrentPlayerState.Attack();
            else if (_direction.y > 0) _player.CurrentPlayerState.Attack(PlayerCombat.AttackModifiers.UP_ARROW);
            else if (_direction.y < 0) _player.CurrentPlayerState.Attack(PlayerCombat.AttackModifiers.DOWN_ARROW);
        }
    }

    private void OnDownArrowModifier(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        isDownArrowPressed = value.Get<float>() == 1 ? true : false;
    }
    private void OnControlShadow(InputValue value)
    {
        Logger.Log(value.Get<float>());
        if (PauseSettings.IsGamePaused) return;
        if (isDownArrowPressed) shadowModifier = ShadowControlInputs.ENTER;
        else if (_horizontalModifier != 0) shadowModifier = ShadowControlInputs.SHADOW_SPIKE;
        else shadowModifier = (ShadowControlInputs)value.Get<float>();
        if (_useCommands)
        {
            _inputStack.CurrentCommand = new ShadowControlInputCommand(_player.CurrentPlayerState, shadowModifier);
        }
        else
        {
            _player.CurrentPlayerState.ControlShadow(shadowModifier);

        }
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
}
