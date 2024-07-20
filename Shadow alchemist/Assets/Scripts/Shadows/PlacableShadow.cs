using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableShadow : MonoBehaviour
{
    [SerializeField] float _shadowBarCost;
    [SerializeField] Rigidbody2D _rb;
    public void Move(Vector2 direction)
    {
        _rb.MovePosition(_rb.position+direction * Time.deltaTime);
    }
}
