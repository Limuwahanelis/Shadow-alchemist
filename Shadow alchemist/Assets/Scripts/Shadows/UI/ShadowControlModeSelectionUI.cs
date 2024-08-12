using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowControlModeSelectionUI : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] protected GameObject _panel;
    [SerializeField] protected GameObject _transmutateSahdowHighlight;
    [SerializeField] protected GameObject _transmutateHorizontalShadowIcon;
    [SerializeField] protected GameObject _transmutateVerticalShadowIcon;
    [SerializeField] protected GameObject _moveHorizontalShadowIcon;
    [SerializeField] protected GameObject _moveVerticalShadowIcon;
    [SerializeField] protected GameObject _moveShadowHighlight;
    private bool _isShadowHorizontal = true;
    public virtual void SetVisiblity(bool visiblity)
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
    public virtual void SelectShadowTransmutation()
    {
        _moveShadowHighlight.SetActive(false);
        _transmutateSahdowHighlight.SetActive(true);
    }
    public virtual void SelectMoveShadow()
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
