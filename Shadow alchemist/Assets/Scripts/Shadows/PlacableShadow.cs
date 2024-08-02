using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableShadow : MonoBehaviour
{
    public Action<PlacableShadow> OnMaxTimeReached;
    //public Action<PlacableShadow> OnLeftParentShadow;
    private bool _isPlaced = false;
    public float ShadowBarCost => _shadowBarCost;
    public bool CanBePlaced
    {
        get
        {
            if(_isInFullShadow)
            {
                if(_collidersInside.Count==0) return true;
            }
            else if(_collidersInside.Count==1 && _collidersInside[0] == _shadowParentCol) return true;
            return false;
        }
    }
    [SerializeField] float _shadowBarCost;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Collider2D _col;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Color _wrongColor;
    private bool _isInFullShadow;
    private Color _originalColor;
    private ControllableShadow _parentShadow;
    private Collider2D _shadowParentCol;
    private List<Collider2D> _collidersInside= new List<Collider2D>();
    private Coroutine _palcementCor;
    private Coroutine _timeLimitCor;
    private void Start()
    {
        _originalColor = _spriteRenderer.color;
    }
    private void Update()
    {
        if (CanBePlaced)
        {
            _spriteRenderer.color = _originalColor;
        }
        else _spriteRenderer.color = _wrongColor;
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
    public void SetFullShadow()
    {
        _isInFullShadow = true;
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
    public void StartTimeLimit()
    {
        _timeLimitCor=StartCoroutine(TimeLimit());
    }
    public void ForceDestroy()
    {
        if(_timeLimitCor!=null)
        {
            StopCoroutine(_timeLimitCor);
            _timeLimitCor = null;
        }
        OnMaxTimeReached?.Invoke(this);
    }
    IEnumerator TimeLimit()
    {
        yield return new WaitForSeconds(3f);
        OnMaxTimeReached?.Invoke(this);

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
