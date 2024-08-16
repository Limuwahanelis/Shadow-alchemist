using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowPlacingState : PlayerState
{
    private bool _isSelectingPlacableShadow = true;
    private bool _isPlacingShadow = false;
    public static Type StateType { get => typeof(PlayerShadowPlacingState); }
    public PlayerShadowPlacingState(GetState function) : base(function)
    {
    }

    public override void Update()
    {
        PerformInputCommand();
    }

    public override void Move(Vector2 direction)
    {
        if(_isPlacingShadow) _context.shadowControl.ShadowToPlace.Move(direction*_context.shadowControl.ShadowPlacingSpeed*Time.deltaTime);

    }

    public override void SetUpState(PlayerContext context)
    {
        base.SetUpState(context);
        ShowShadowSelection();
    }
    public override void Attack(PlayerCombat.AttackModifiers modifier)
    {
        if (_isSelectingPlacableShadow)
        {
             if (_context.shadowControl.FullShadow)
            {
                _isSelectingPlacableShadow = false;
                _context.placableShadowSelection.SelectShadow();
                _context.shadowControl.PlacingShadowDespawned += ForcedShadowDespawn;
                _context.placableShadowSelection.SetSelectionVisibility(false);
                _isPlacingShadow = true;
            }
            else if (_context.shadowControl.Shadow.TakenShadowBarValueFromTransmutation >= _context.placableShadowSelection.CurrentlyHighlihtedShadow.ShadowBarCost)
            {
                // select which shadow to spawn, spawn it but not place it yet
                _isSelectingPlacableShadow = false;
                _context.placableShadowSelection.SelectShadow();
                _context.shadowControl.PlacingShadowDespawned += ForcedShadowDespawn;
                _context.placableShadowSelection.SetSelectionVisibility(false);
                _isPlacingShadow = true;
            }

        }
        else if(_isPlacingShadow)
        {
            if(_context.shadowControl.PlaceShadow())
            {
                // place shadow
                ShowShadowSelection();
                _isPlacingShadow = false;
                _isSelectingPlacableShadow = true;
            }
        }

    }
    public override void Jump()
    {
        if (_isSelectingPlacableShadow)
        {
            if (_context.shadowControl.FullShadow) return;
            _context.shadowControl.Shadow.RemoveRecentShadow();
        }
        else
        {
            _context.shadowControl.DespawnShadow();
            ShowShadowSelection();
        }
    }
    public override void ControlShadow(PlayerInputHandler.ShadowControlInputs controlInput)
    {
        _context.shadowControl.DespawnShadow();
        _context.placableShadowSelection.SetSelectionVisibility(false);
        if (_context.shadowControl.FullShadow)
        {
            ChangeState(PlayerIdleState.StateType);
            return;
        }
        ChangeState(PlayerShadowControlState.StateType);
    }
    public override void InterruptState()
    {
        _context.shadowControl.DespawnShadow();
    }
    private void ShowShadowSelection()
    {
        _context.placableShadowSelection.SetSelectionVisibility(true);
        _isSelectingPlacableShadow = true;
        _isPlacingShadow = false;
    }
    private void ForcedShadowDespawn()
    {
        _context.shadowControl.PlacingShadowDespawned -= ForcedShadowDespawn;
        ShowShadowSelection();

    }
}