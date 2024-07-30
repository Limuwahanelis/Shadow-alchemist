using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float ChargeSpeed => _chargeSpeed;
    public GlobalEnums.HorizontalDirections FlipSide => (GlobalEnums.HorizontalDirections)_flipSide;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _chargeSpeed;
    [SerializeField] BossController _boss;
    [SerializeField] Collider2D _col;
    [Header("push"), SerializeField] Ringhandle _pushHandle;
    [SerializeField] float _pushForce;
    private int _flipSide = -1;
    Vector3 _mainbodyScale;
    private void Start()
    {
        _mainbodyScale = _boss.MainBody.transform.localScale;
    }
    public void FlipEnemy()
    {
        _mainbodyScale.x = -_mainbodyScale.x;
        _mainbodyScale.y = _boss.MainBody.transform.localScale.y;
        _mainbodyScale.z = _boss.MainBody.transform.localScale.z;
        _boss.MainBody.transform.localScale = _mainbodyScale;
        _flipSide = -_flipSide;
    }
    public void Move(Vector2 direction)
    {
        _rb.MovePosition(_rb.position + direction * _chargeSpeed * Time.deltaTime);
    }
    public void SetForTeleprt()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _col.enabled = false;
    }
    public void SetRb(RigidbodyType2D type)
    {
        _rb.bodyType = type;
    }
    public void SetCollider(bool enabled)
    {
        _col.enabled = enabled;
    }
    public void Stop()
    {
        _rb.velocity = Vector2.zero;
    }
}
