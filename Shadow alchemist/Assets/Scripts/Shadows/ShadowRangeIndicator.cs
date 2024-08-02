using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
public class ShadowRangeIndicator : MonoBehaviour
{
    [SerializeField] GameObject _inGameIndicator;
    [SerializeField] ControllableShadow _shadow;
    [SerializeField] bool _isRightBorder;
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
    public void SetRange(Vector3 maxMoveDistance)
    {
        transform.position += maxMoveDistance;
        transform.position -= transform.localScale / 2;
    }
}
