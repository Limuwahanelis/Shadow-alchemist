using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableShadow : MonoBehaviour
{
    public Action OnLeftParentShadow;
    public bool CanBePlaced
    {
        get
        {
            if(_collidersInside.Count==1)return true;
            return false;
        }
    }
    [SerializeField] float _shadowBarCost;
    [SerializeField] Rigidbody2D _rb;
    private ControllableShadow _parentShadow;
    private Collider2D _shadowParentCol;
    private List<Collider2D> _collidersInside= new List<Collider2D>();
    public void Move(Vector2 direction)
    {
        _rb.MovePosition(_rb.position+direction * Time.deltaTime);
    }

    public void SetParentShadow(ControllableShadow shadow,Collider2D shadwoCol)
    {
        _parentShadow = shadow;
        _shadowParentCol = shadwoCol;
        _collidersInside.Add(shadwoCol);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_collidersInside.Contains(collision))
        {
            _collidersInside.Add(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_collidersInside.Contains(collision))
        {
            _collidersInside.Remove(collision);
        }
        if (collision== _shadowParentCol)
        {
            OnLeftParentShadow?.Invoke();
        }
    }
}
