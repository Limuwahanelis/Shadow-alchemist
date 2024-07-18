using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseSetter : MonoBehaviour
{
    [SerializeField] InputActionReference _playerPause;
    public UnityEvent OnPause;
    public UnityEvent OnResume;
    private static bool _isPauseForced;
    public void SetPause(bool value)
    {
        if (_isPauseForced) return;
        GlobalSettings.SetGamePause(value);
        if (value) OnPause?.Invoke();
        else OnResume?.Invoke();
    }

    public void SetForcedPause(bool value)
    {
        if (value) _playerPause.action.Disable();
        else _playerPause.action.Enable();
        _isPauseForced = value;
        GlobalSettings.SetGamePause(value);
        if (value) OnPause?.Invoke();
        else OnResume?.Invoke();
    }

}
