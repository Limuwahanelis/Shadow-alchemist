using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    public enum ShadowControlInputs
    {
        CONTROL=1, TRANSMUTATE,MOVE,PLACE
    }
    [SerializeField] PlayerController _player;
    [SerializeField] InputActionAsset _controls;
    [SerializeField] bool _useCommands;
    [SerializeField] PlayerInputStack _inputStack;
    //private PlayerInteract _playerInteract;
    private bool isDownArrowPressed;
    private Vector2 _direction;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();
        //_playerInteract = GetComponent<PlayerInteract>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.IsAlive)
        {

            if (!GlobalSettings.IsGamePaused)
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
        if (GlobalSettings.IsGamePaused) return;
        if (_useCommands) _inputStack.CurrentCommand= new JumpInputCommand(_player.CurrentPlayerState);
        else _player.CurrentPlayerState.Jump();
        //if (direction * _player.mainBody.transform.localScale.x > 0 && isDownArrowPressed) _player.currentState.Slide();

    }
    void OnVertical(InputValue value)
    {
        _direction = value.Get<Vector2>();
        Logger.Log(_direction);
    }

    private void OnAttack(InputValue value)
    {
        _player.CurrentPlayerState.Attack();
        //if (_useCommands)
        //{
        //    _inputStack.CurrentCommand = new AttackInputCommand(_player.CurrentPlayerState);
        //    if (_direction.y > 0)  _inputStack.CurrentCommand = new AttackInputCommand(_player.CurrentPlayerState, PlayerCombat.AttackModifiers.UP_ARROW);
        //    if (_direction.y < 0)  _inputStack.CurrentCommand = new AttackInputCommand(_player.CurrentPlayerState, PlayerCombat.AttackModifiers.DOWN_ARROW);
        //}
        //else
        //{
        //    if (GlobalSettings.IsGamePaused) return;
        //    if (_direction.y > 0) _player.CurrentPlayerState.Attack(PlayerCombat.AttackModifiers.UP_ARROW);
        //    if (_direction.y < 0) _player.CurrentPlayerState.Attack(PlayerCombat.AttackModifiers.DOWN_ARROW);
        //    else _player.CurrentPlayerState.Attack();
        //}
    }

    private void OnDownArrowModifier(InputValue value)
    {
        if (GlobalSettings.IsGamePaused) return;
        isDownArrowPressed = value.Get<float>() == 1 ? true : false;
    }
    private void OnControlShadow(InputValue value)
    {
        Debug.Log(value.Get<float>());
        if (GlobalSettings.IsGamePaused) return;
        if (_useCommands) _inputStack.CurrentCommand = new ShadowControlInputCommand(_player.CurrentPlayerState, (ShadowControlInputs)value.Get<int>());
        else _player.CurrentPlayerState.ControlShadow((ShadowControlInputs)value.Get<float>());
    }
    private void OnInteract(InputValue value)
    {
        if (GlobalSettings.IsGamePaused) return;
        //_playerInteract.InteractWithObject();
    }
    private void OnWarp(InputValue value)
    {
        if (GlobalSettings.IsGamePaused) return;
        _player.CurrentPlayerState.Dodge();
    }
}
