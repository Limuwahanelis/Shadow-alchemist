using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum playerDirection
    {
        LEFT = -1,
        SAME = 0,
        RIGHT = 1
    }
    public enum PhysicMaterialType
    {
        NONE,NO_FRICTION,NORMAL
    }
    public float DodgeSpeed=>_dodgeSpeed;
    public bool IsPlayerFalling { get=>_rb.velocity.y<0; }
    public Rigidbody2D PlayerRB =>_rb;
    public int FlipSide => _flipSide;
    private playerDirection _newPlayerDirection;
    private playerDirection _oldPlayerDirection;
    [SerializeField] float _playerSpeed;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] PlayerController _player;
    [SerializeField] float _dodgeSpeed;
    
    [Header("Push")]
    [SerializeField] Ringhandle _pushHandle;
    [SerializeField] float _pushForce;
    [Header("Jump")]
    [SerializeField] float _jumpStrength;
    [SerializeField] float _jumpSpeedMultiplier;
    [SerializeField] PhysicsMaterial2D _noFrictionMat;
    [SerializeField] PhysicsMaterial2D _normalMaterial;
    private float _previousDirection;
    private int _flipSide = 1;
    public void Move(float direction,bool inAir=false)
    {
        if (direction != 0)
        {
            _oldPlayerDirection = _newPlayerDirection;
            _newPlayerDirection = (playerDirection)direction;
            _rb.velocity = new Vector3(direction * _playerSpeed*(inAir?_jumpSpeedMultiplier:1), _rb.velocity.y, 0);
            if (direction > 0)
            {
                _flipSide = 1;
                _player.MainBody.transform.localScale = new Vector3(_flipSide, _player.MainBody.transform.localScale.y, _player.MainBody.transform.localScale.z);
            }
            if (direction < 0)
            {
                _flipSide = -1;
                _player.MainBody.transform.localScale = new Vector3(_flipSide, _player.MainBody.transform.localScale.y, _player.MainBody.transform.localScale.z);
            }

        }
        else
        {
            if (_previousDirection != 0)
            {
                _rb.velocity = new Vector2(0, _rb.velocity.y);
                //StopPlayerOnXAxis();
            }
        }
        _previousDirection = direction;
    }
    public void StopPlayer(bool vertical=true,bool horizontal = true)
    {
        if (horizontal) _rb.velocity = new Vector2(0,_rb.velocity.y);
        if (vertical) _rb.velocity = new Vector2(_rb.velocity.x, 0);
    }
    public void Jump()
    {
        _rb.velocity = new Vector3(0, 0, 0);
        _rb.AddForce(new Vector2(0, _jumpStrength),ForceMode2D.Impulse);
    }
    public void Dodge()
    {
        _rb.velocity = new Vector3(FlipSide * _dodgeSpeed, _rb.velocity.y, 0);
    }
    public void MoveRB(Vector2 pos)
    {
        _rb.MovePosition(pos);
    }

    public void SetRBMaterial(PhysicMaterialType type)
    {
        switch(type)
        {
            case PhysicMaterialType.NORMAL: _rb.sharedMaterial=_normalMaterial; break;
            case PhysicMaterialType.NONE: _rb.sharedMaterial = null; break;
            case PhysicMaterialType.NO_FRICTION: _rb.sharedMaterial = _noFrictionMat;break;
        }
    }
    public void SetRB(bool isdynamic)
    {
        if(isdynamic) _rb.bodyType=RigidbodyType2D.Dynamic;
        else _rb.bodyType = RigidbodyType2D.Kinematic;
    }
    public void SetRBVelocity(Vector2 velocity)
    {
        _rb.velocity=velocity;
    }
    public void PushPlayer(PushInfo pushInfo)
    {
        StopPlayer();
        Vector2 pushDirection= _pushHandle.GetVector();
        
        if (!pushInfo.pushType.HasFlag(HealthSystem.DamageType.TRAPS))
        {
            if (HelperClass.CheckIfObjectIsBehind(transform.position, pushInfo.pushPosition, (GlobalEnums.HorizontalDirections)_flipSide)) pushDirection.x = -pushDirection.x;
        }
        _rb.AddForce(pushDirection * _pushForce, ForceMode2D.Impulse);

    }
}
