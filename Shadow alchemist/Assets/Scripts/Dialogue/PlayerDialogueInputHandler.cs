using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDialogueInputHandler : MonoBehaviour
{
    [SerializeField] InputActionReference _confirmSelection;
    [SerializeField] InputActionReference _selectNextOption;
    [SerializeField] InputActionReference _selectPreviousOption;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerControlsControl _controls;
    private ConversationManager _conversationManager;
    public void StartConversation(ConversationManager manager)
    {
        SetConversationManager(manager);
        _playerMovement.StopPlayer();
        _controls.SetPlayerInput(false);
    }
    private void SetConversationManager(ConversationManager manager)
    {
        _conversationManager = manager;
        _selectNextOption.action.performed += SelectNextOption;
        _selectPreviousOption.action.performed += SelectPreviousOption;
        _confirmSelection.action.performed += ConfirmSelection;
        _confirmSelection.action.Enable();
        _selectNextOption.action.Enable();
        _selectPreviousOption.action.Enable();


    }

    private void SelectNextOption(InputAction.CallbackContext context)
    {
        _conversationManager.SelectNextOption();
    }
    private void SelectPreviousOption(InputAction.CallbackContext context)
    {
        _conversationManager.SelectPreviousOption();
    }
    private void ConfirmSelection (InputAction.CallbackContext context)
    {
        _conversationManager.PressSelectedOption();
    }

    public void EndConversation()
    {
        _conversationManager = null;
        _selectNextOption.action.performed -= SelectNextOption;
        _selectPreviousOption.action.performed -= SelectPreviousOption;
        _confirmSelection.action.performed -= ConfirmSelection;
        _confirmSelection.action.Disable();
        _selectNextOption.action.Disable();
        _selectPreviousOption.action.Disable();
        _controls.SetPlayerInput(true);
    }
}
