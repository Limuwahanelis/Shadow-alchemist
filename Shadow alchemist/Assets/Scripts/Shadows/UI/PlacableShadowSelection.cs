using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacableShadowSelection : MonoBehaviour
{
    public PlacableShadow CurrentlyHighlihtedShadow => _currentlyHighlihtedShadow;
    [SerializeField] List<PlacableShadowIcon> _icons= new List<PlacableShadowIcon>();

    [SerializeField] InputActionReference _selectLeftIconActionRef;
    [SerializeField] InputActionReference _selectRightIconActionRef;
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _panel;
    private PlacableShadow _currentlyHighlihtedShadow;
    private int _index=0;
    private void Awake()
    {

        _icons[_index].SelectIcon();
        _currentlyHighlihtedShadow = _icons[_index].ShadowPrefab.GetComponent<PlacableShadow>();
    }
    private void OnEnable()
    {
        _selectLeftIconActionRef.action.Enable();
        _selectRightIconActionRef.action.Enable();
        _selectLeftIconActionRef.action.started += SelectLeftIcon;
        _selectRightIconActionRef.action.started += SelectRightIcon;
    }
    public void SetSelectionVisibility(bool isVisible)
    {
        _panel.SetActive(isVisible);
        _canvas.enabled = isVisible;
        enabled = isVisible;
    }
    public virtual void SelectShadow()
    {
        _icons[_index].SelectShadow();
    }
    private void SelectLeftIcon(InputAction.CallbackContext context)
    {
        _index -= 1;
        _index = math.clamp(_index,0, _icons.Count-1);
        _icons[_index+1].DeselectIcon();
        _icons[_index].SelectIcon();
        _currentlyHighlihtedShadow = _icons[_index].ShadowPrefab.GetComponent<PlacableShadow>();
    }
    private void SelectRightIcon(InputAction.CallbackContext context)
    {
        _index += 1;
        _index = math.clamp(_index, 0, _icons.Count-1);
        _icons[_index - 1].DeselectIcon();
        _icons[_index].SelectIcon();
    }
    private void OnDisable()
    {
        _selectLeftIconActionRef.action.Disable();
        _selectRightIconActionRef.action.Disable();
        _selectLeftIconActionRef.action.performed -= SelectLeftIcon;
        _selectRightIconActionRef.action.performed -= SelectRightIcon;
    }
}
