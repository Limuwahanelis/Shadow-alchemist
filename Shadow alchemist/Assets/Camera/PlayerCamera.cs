using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform _transformToFollow;
    public Vector3 offset;


    public bool CheckForBorders = true;
    public Transform leftScreenBorder;
    public Transform rightScreenBorder;
    public Transform upperScreenBorder;
    public Transform lowerScreenBorder;

    public float smoothTime = 0.3f;

    private bool _followOnXAxis=true;
    private bool _followOnYAxis = true;
    private Vector3 _targetPos;
    
    private Vector3 _velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = _transformToFollow.position+offset;
    }
    private void Update()
    {
        if (CheckForBorders)
        {
            if (_transformToFollow.position.x < leftScreenBorder.position.x)
            {
                _followOnXAxis = false;
                _targetPos = new Vector3(leftScreenBorder.position.x, _transformToFollow.position.y);
            }
            else
            {
                CheckIfPlayerIsOnRightScreenBorder();
            }

            if (_transformToFollow.position.y < lowerScreenBorder.position.y)
            {
                _followOnYAxis = false;
                _targetPos = new Vector3(_targetPos.x, lowerScreenBorder.position.y, _targetPos.z);

            }
            else
            {
                CheckIfPlayerIsOnUpperScreenBorder();
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (CheckForBorders)
        {
            if (_followOnXAxis)
            {
                _targetPos = new Vector3(_transformToFollow.position.x, _targetPos.y);
            }
            if (_followOnYAxis)
            {
                _targetPos = new Vector3(_targetPos.x, _transformToFollow.position.y);
            }
        }
        else
        {
            _targetPos = _transformToFollow.position;
        }
            _targetPos += offset;
            transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref _velocity, smoothTime);
    }

    private void CheckIfPlayerIsOnRightScreenBorder()
    {
        if (_transformToFollow.position.x > rightScreenBorder.position.x)
        {
            _followOnXAxis = false;
            _targetPos = new Vector3(rightScreenBorder.position.x, _transformToFollow.position.y);
        }
        else
        {
            _followOnXAxis = true;
        }
    }
    private void CheckIfPlayerIsOnUpperScreenBorder()
    {
        if (_transformToFollow.position.y > upperScreenBorder.position.y)
        {
            _followOnYAxis = false;
            _targetPos = new Vector3(_targetPos.x, upperScreenBorder.position.y,_targetPos.z);
        }
        else
        {
            _followOnYAxis = true;
        }
    }
}
