using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowControlModeSelectionUI : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _panel;
    [SerializeField] GameObject _transmutateSahdowHighlight;
    [SerializeField] GameObject _moveShadowHighlight;
    public void SetVisiblity(bool visiblity)
    {
        _panel.SetActive(visiblity);
        _canvas.enabled = visiblity;
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
