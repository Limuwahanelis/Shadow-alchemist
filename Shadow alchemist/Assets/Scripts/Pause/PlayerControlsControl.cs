using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlsControl : MonoBehaviour
{
    [SerializeField] PlayerInput _payerInput;
    public void SetPlayerInput(bool isEnabled)
    {
        _payerInput.enabled = isEnabled;
    }
}
