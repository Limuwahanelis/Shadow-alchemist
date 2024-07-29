using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

public class ShadowBar : MonoBehaviour
{
    public float CurrentValue => _currentValue;
    [SerializeField] GameObject _fill;
    [SerializeField] float _max;
    [SerializeField] float _min;
    [SerializeField] float _scaleAtMax;
    [SerializeField] float _posValueAtMin;
    [SerializeField] float _posValueAtMax;
    [SerializeField] float _currentValue=0;
    public void SetVisibility(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetValue(float newValue)
    {
        Vector2 scale = _fill.transform.localScale;
        Vector2 position = _fill.transform.localPosition;
        newValue=math.clamp(newValue, 0, _max);
        float remappedValue = math.remap(0, _max, 0, _scaleAtMax, newValue);
        float mappedPos = math.remap(0, _scaleAtMax, _posValueAtMin, _posValueAtMax, remappedValue);
        scale.y = remappedValue;
        position.y = mappedPos;
        _fill.transform.localScale = scale;
        _fill.transform.localPosition = position;
        _currentValue = newValue;
    }
    private void OnValidate()
    {
        _currentValue=math.clamp(_currentValue, 0, _max);
        SetValue(_currentValue);
    }
}
