using MyBox;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class ShadowRangeIndicator : MonoBehaviour
{
    [SerializeField] GameObject _inGameIndicator;
    [SerializeField] ControllableShadow _shadow;
    [SerializeField] bool _isHorizontal = true;
    [SerializeField,ConditionalField(nameof(_isHorizontal))] bool _isRightBorder;
    [SerializeField, ConditionalField(nameof(_isHorizontal),inverse: true)] bool _isUpperBorder;
    private void Start()
    {
        SetVisibility(false);
    }
    public void SetVisibility(bool isVisible)
    {
        _inGameIndicator.SetActive(isVisible);
    }
    public void SetXPos(float xPosition)
    {
        Vector3 pos = _inGameIndicator.transform.position;
        pos.x= xPosition;
        pos.x-= _inGameIndicator.transform.localScale.x / 2;
        _inGameIndicator.transform.position = pos;
    }
    public void SetYPos(float yPosition)
    {
        Vector3 pos = _inGameIndicator.transform.position;
        pos.y = yPosition;
        pos.y -= _inGameIndicator.transform.localScale.x / 2;
        _inGameIndicator.transform.position = pos;
    }
    public void SetRange(Vector3 maxMoveDistance)
    {
        transform.position += maxMoveDistance;
        transform.position -= transform.localScale / 2;
    }
}
