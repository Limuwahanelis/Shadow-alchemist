using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterMovement : MonoBehaviour
{

    public GlobalEnums.HorizontalDirections FlipSide => (GlobalEnums.HorizontalDirections)_flipSide;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _speed;
    [SerializeField] ShadowFighterController _shadowFighter;
    [Header("push"),SerializeField] Ringhandle _pushHandle;
    [SerializeField] float _pushForce;
    private int _flipSide = 1;
    Vector3 _mainbodyScale;
    public void Move(Vector2 direction)
    {
        if (direction.x > 0)
        {
            _mainbodyScale.x = 1;
            _mainbodyScale.y = _shadowFighter.MainBody.transform.localScale.y;
            _mainbodyScale.z = _shadowFighter.MainBody.transform.localScale.z;
            _shadowFighter.MainBody.transform.localScale = _mainbodyScale;
            _flipSide = 1;
        }
        else
        {
            _mainbodyScale.x = -1;
            _mainbodyScale.y = _shadowFighter.MainBody.transform.localScale.y;
            _mainbodyScale.z = _shadowFighter.MainBody.transform.localScale.z;
            _shadowFighter.MainBody.transform.localScale = _mainbodyScale;
            _flipSide = -1;
        }
        _rb.MovePosition(_rb.position + direction * _speed * Time.deltaTime);
    }
    public void FlipEnemy()
    {
        _mainbodyScale.x = -_mainbodyScale.x;
        _mainbodyScale.y = _shadowFighter.MainBody.transform.localScale.y;
        _mainbodyScale.z = _shadowFighter.MainBody.transform.localScale.z;
        _shadowFighter.MainBody.transform.localScale = _mainbodyScale;
        _flipSide = -_flipSide;
    }
    public void Stop()
    {
        _rb.velocity = Vector3.zero;
    }
    public void Push(float direction=1)
    {
        Vector2 pushvector = _pushHandle.GetVector();
        pushvector.x *= direction;
        _rb.AddForce(pushvector * _pushForce,ForceMode2D.Impulse);
    }
}
