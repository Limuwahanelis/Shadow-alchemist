using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RestrictAllActionsButOne : MonoBehaviour
{
    InputActionReference _inputActionToPreserve;

    public void DisableActions(InputActionReference inputActionToPreserve)
    {
        _inputActionToPreserve = inputActionToPreserve;
        inputActionToPreserve.action.actionMap.Disable();
        inputActionToPreserve.action.Enable();
    }
    public void EnableActions()
    {
        _inputActionToPreserve.action.actionMap.Enable();
    }
}
