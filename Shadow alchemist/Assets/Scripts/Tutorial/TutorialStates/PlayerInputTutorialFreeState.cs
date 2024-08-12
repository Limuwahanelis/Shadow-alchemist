using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandler;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.InputSystem;

public class PlayerInputTutorialFreeState : PlayerInputTutorialState
{
    public PlayerInputTutorialFreeState(GetState getState, bool useCommands, PlayerInputStack inputStack) : base(getState, useCommands, inputStack)
    {
    }

    override public void Update()
    {
        if (_context.playerController.IsAlive)
        {

            if (!PauseSettings.IsGamePaused)
            {
                _context.playerController.CurrentPlayerState.Move(_context.direction);

            }
        }
    }
    override public void OnMove(InputValue value)
    {
        _context.direction = value.Get<Vector2>();

    }
    override public void OnJump(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        if (_useCommands) _inputStack.CurrentCommand = new JumpInputCommand(_context.playerController.CurrentPlayerState);
        else _context.playerController.CurrentPlayerState.Jump();
        //if (direction * _player.mainBody.transform.localScale.x > 0 && isDownArrowPressed) _player.currentState.Slide();

    }
    public override void OnVertical(InputValue value)
    {
        _context.direction = value.Get<Vector2>();
    }
    public override void OnHorizontalModifier(InputValue value)
    {
        _context.horizontalModifier = value.Get<float>();
    }
    public override void OnAttack(InputValue value)
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

    public override void OnDownArrowModifier(InputValue value)
    {
        if (PauseSettings.IsGamePaused) return;
        _context.isDownArrowPressed = value.Get<float>() == 1 ? true : false;
    }
    public override void OnControlShadow(InputValue value)
    {
        Logger.Log(value.Get<float>());
        if (PauseSettings.IsGamePaused) return;
        if (_context.isDownArrowPressed) _context.shadowModifier = ShadowControlInputs.ENTER;
        else if (_context.horizontalModifier != 0) _context.shadowModifier = ShadowControlInputs.SHADOW_SPIKE;
        else _context.shadowModifier = (ShadowControlInputs)value.Get<float>();
        if (_useCommands)
        {
            _inputStack.CurrentCommand = new ShadowControlInputCommand(_context.playerController.CurrentPlayerState, _context.shadowModifier);
        }
        else
        {
            _context.playerController.CurrentPlayerState.ControlShadow(_context.shadowModifier);

        }
    }


}
