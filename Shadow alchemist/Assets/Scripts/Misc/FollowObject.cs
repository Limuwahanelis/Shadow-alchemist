using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform _objectToFollow;
    Vector3 _offset;
    // Update is called once per frame
    private void Start()
    {
        _offset=_objectToFollow.position-transform.position;
    }
    void Update()
    {
        transform.position = _objectToFollow.position-_offset;
    }
}
