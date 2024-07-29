using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableShadow : MonoBehaviour
{
    public Action<PlacableShadow> OnLeftParentShadow;
    public float ShadowBarCost => _shadowBarCost;
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
    [SerializeField] Collider2D _col;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Color _wrongColor;
    private Color _originalColor;
    private ControllableShadow _parentShadow;
    private Collider2D _shadowParentCol;
    private List<Collider2D> _collidersInside= new List<Collider2D>();
    private Coroutine _palcementCor;
    private void Start()
    {
        _originalColor = _spriteRenderer.color;
    }
    public void Move(Vector2 direction)
    {
        _rb.MovePosition(_rb.position+direction);
    }
    public void ChageTriggerToCol()
    {
        _col.isTrigger = false;
        if (_palcementCor != null)
        {
            StopCoroutine(_palcementCor); _palcementCor = null;
            _spriteRenderer.color = _originalColor;
        }

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
            OnLeftParentShadow?.Invoke(this);
        }
    }
    public void StartCantPlaceCor()
    {
        if(_palcementCor!=null)
        {
            StopCoroutine(_palcementCor);
            _palcementCor = null;
        }
        _palcementCor = StartCoroutine(CantPlaceShadowCor());
    }
    IEnumerator CantPlaceShadowCor()
    {
        float _time = 0;

        _spriteRenderer.color = _wrongColor;
        while (_time < 2)
        {
            _spriteRenderer.color = Color.Lerp(_wrongColor, _originalColor, _time / 2);
            _time += Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.color = _originalColor;
    }
}
