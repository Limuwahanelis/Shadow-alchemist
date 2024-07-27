using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowControlModeSelectionUI : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _panel;
    [SerializeField] GameObject _transmutateSahdowHighlight;
    [SerializeField] GameObject _transmutateHorizontalShadowIcon;
    [SerializeField] GameObject _transmutateVerticalShadowIcon;
    [SerializeField] GameObject _moveHorizontalShadowIcon;
    [SerializeField] GameObject _moveVerticalShadowIcon;
    [SerializeField] GameObject _moveShadowHighlight;
    private bool _isShadowHorizontal = true;
    public void SetVisiblity(bool visiblity)
    {
        

        _panel.SetActive(visiblity);
        _canvas.enabled = visiblity;
    }
    public void SetShadowType(bool isShadowHorizontal)
    {
        this._isShadowHorizontal = isShadowHorizontal;
        if (_isShadowHorizontal)
        {
            _transmutateHorizontalShadowIcon.SetActive(true);
            _moveHorizontalShadowIcon.SetActive(true);
            _transmutateVerticalShadowIcon.SetActive(false);
            _moveVerticalShadowIcon.SetActive(false);
        }
        else
        {
            _transmutateHorizontalShadowIcon.SetActive(false);
            _moveHorizontalShadowIcon.SetActive(false);
            _transmutateVerticalShadowIcon.SetActive(true);
            _moveVerticalShadowIcon.SetActive(true);
        }
    }
    public void SelectShadowTransmutation()
    {
        _moveShadowHighlight.SetActive(false);
        _transmutateSahdowHighlight.SetActive(true);
    }
    public void SelectMoveShadow()
    {
        _moveShadowHighlight.SetActive(true);
        _transmutateSahdowHighlight.SetActive(false);
    }
    public void Deselect()
    {
        _transmutateSahdowHighlight.SetActive(false);
        _moveShadowHighlight.SetActive(false);
    }
}
