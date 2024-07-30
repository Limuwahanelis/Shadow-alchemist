using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform _objectToFollow;
    [SerializeField] bool _excludeYAxis;
    Vector3 _offset;
    Vector3 _pos;
    // Update is called once per frame
    private void Start()
    {
        _offset=_objectToFollow.position-transform.position;
    }
    void Update()
    {
        if ((_excludeYAxis))
        {
            _pos = _objectToFollow.position - _offset;
            _pos.y = transform.position.y;
            transform.position = _pos;
        }
        else transform.position = _objectToFollow.position - _offset;

    }
}
