using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacableShadowSelection : MonoBehaviour
{
    [SerializeField] List<PlacableShadowIcon> _icons= new List<PlacableShadowIcon>();

    [SerializeField] InputActionReference _selectLeftIconActionRef;
    [SerializeField] InputActionReference _selectRightIconActionRef;
    [SerializeField] Canvas _canvas;
    private int _index=0;
    private void Awake()
    {

        _icons[_index].SelectIcon();
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
        _canvas.enabled = isVisible;
        enabled = isVisible;
    }
    public void SelectShadow()
    {
        _icons[_index].SelectShadow();
    }
    private void SelectLeftIcon(InputAction.CallbackContext context)
    {
        _index -= 1;
        _index = math.clamp(_index,0, _icons.Count-1);
        _icons[_index+1].DeselectIcon();
        _icons[_index].SelectIcon();
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
