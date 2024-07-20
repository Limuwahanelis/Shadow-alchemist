using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlacableShadowIcon : MonoBehaviour
{
    public UnityEvent<GameObject> OnShadowPicked;
    public GameObject ShadowPrefab => _placableShadowPrefab;
    [SerializeField] GameObject _placableShadowPrefab;
    [SerializeField] Color _selectedColor;
    [SerializeField] Image _image;
    private Color startingColor = Color.white;

    public void DeselectIcon()
    {
        _image.color = startingColor;
    }
    public void SelectIcon()
    {
        _image.color = _selectedColor;
    }
    public void SelectShadow()
    {
        OnShadowPicked?.Invoke(_placableShadowPrefab);
    }
}
