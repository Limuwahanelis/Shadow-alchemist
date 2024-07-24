using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _speed;
    [SerializeField] ShadowFighterController _shadowFighter;
    //private int _flipSide = 1;
    Vector3 _mainbodyScale;
    public void Move(Vector2 direction)
    {
        if (direction.x > 0)
        {
            _mainbodyScale.x = 1;
            _mainbodyScale.y = _shadowFighter.MainBody.transform.localScale.y;
            _mainbodyScale.z = _shadowFighter.MainBody.transform.localScale.z;
            _shadowFighter.MainBody.transform.localScale = _mainbodyScale;
        }
        else
        {
            _mainbodyScale.x = -1;
            _mainbodyScale.y = _shadowFighter.MainBody.transform.localScale.y;
            _mainbodyScale.z = _shadowFighter.MainBody.transform.localScale.z;
            _shadowFighter.MainBody.transform.localScale = _mainbodyScale;
        }
        _rb.MovePosition(_rb.position + direction * _speed * Time.deltaTime);
    }
}
