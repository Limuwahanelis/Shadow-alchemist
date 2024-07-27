using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ControllableShadow : MonoBehaviour, IMovableShadow,ITransmutableSadow
{
    public enum DIR
    {
        NONE=-1,LEFT,UP,RIGHT,DOWN
    }
    public bool IsHorizontal => _isHorizontal;
    public Collider2D ShadowCollider => _shadowCollider;
    public float TakenShadowBarValueFromTransmutation=>_valueForShadowPlacing;
    public Bounds ShadowBounds => _spriteMask.bounds;
    [SerializeField] Transform _shadow;
    [Header("Shadow"),SerializeField] Collider2D _shadowCollider;
    [SerializeField] bool _isHorizontal = true;
    [SerializeField] Transform _shadowMask;
    [SerializeField] float scaleToPoSrate = 2f;
    [SerializeField] float _totalShadowbarValue;
    [SerializeField] float _distanceToResetShadow;
    [SerializeField] CircleCollider2D _resetShadowCollider;
    [Header("Borders"),SerializeField] Transform _lefBorder;
    [SerializeField] Transform _rightBorder;
    [SerializeField] Transform _upperBorder;
    [SerializeField] Transform _lowerBorder;
    [SerializeField] protected SpriteMask _spriteMask;
    [Header("Segments"),SerializeField] List<Transform> _segments;
    private List<PlacableShadow> _placedShadows= new List<PlacableShadow>();
    private float _transmutateValue = 0;
    private float _revertTransmutateValue = 0;
    private float _valueForShadowPlacing;
    private int _segmentsTaken=0;
    private int[] _segmentsTakenPerSide= new int[4] {0,0,0,0 };
    private Vector2 _originalPosition;
    private Coroutine _revertMoveCor;
    private List<DIR> _shadowSegmentsList= new List<DIR>();
    private DIR _lastTransmutationDirection=DIR.NONE;
    private bool _isReverting = false;
    private void Start()
    {
        _originalPosition = _shadow.position;
    }
    #region ShadowMove
    public void MoveShadow(float moveSpeed,Vector2 direction)
    {
        if(_revertMoveCor!=null)
        {
            StopCoroutine(_revertMoveCor);
            _revertMoveCor = null;
        }
        if (_isHorizontal)
        {
            if (direction == Vector2.right)
                if (_shadow.transform.position.x - _shadow.transform.localScale.x / 2 < _lefBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
            if (direction == Vector2.left)
                if (_shadow.transform.position.x + _shadow.transform.localScale.x / 2 > _rightBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
        }
        else
        {
            if (direction == Vector2.up)
                if (_shadow.transform.position.y - _shadow.transform.localScale.y / 2 < _upperBorder.position.y)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
            if (direction == Vector2.down)
                if (_shadow.transform.position.y + _shadow.transform.localScale.y / 2 > _upperBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
        }

    }
    public void RevertMove()
    {
        _revertMoveCor = StartCoroutine(RevertShadowMove());
    }
    IEnumerator RevertShadowMove()
    {
        while (Vector2.Distance(_shadow.transform.position, _originalPosition) > 0.0001)
        {
            Vector2.MoveTowards(_shadow.position, _originalPosition, 3f * Time.deltaTime);
            yield return null;
        }
    }
    #endregion
    #region ShadowPlacing
    public void PlaceNewShadow(PlacableShadow newShadow)
    {
        _placedShadows.Add(newShadow);
        newShadow.OnLeftParentShadow += RemovePlacedShadow;
        _valueForShadowPlacing-=newShadow.ShadowBarCost;
    }
    private void RemovePlacedShadow(PlacableShadow shadow)
    {
        _placedShadows.Remove(shadow);
        shadow.OnLeftParentShadow -= RemovePlacedShadow;
        Destroy(shadow.gameObject);
        
        //_takenVal -= shadow.ShadowBarCos;
    }
    public void RemoveRecentShadow()
    {
        if (_placedShadows.Count == 0) return;
        _placedShadows[_placedShadows.Count - 1].OnLeftParentShadow -= RemovePlacedShadow;
        RevertTransmutationFromPlacedShadow(_placedShadows[_placedShadows.Count - 1].ShadowBarCost);
        Destroy(_placedShadows[_placedShadows.Count - 1].gameObject);
        _placedShadows.RemoveAt(_placedShadows.Count - 1);
        
    }
    #endregion

    public void Transmutate(Vector2 directionTotakeFrom)
    {

        Vector2 newScale = _shadowMask.localScale;
        Vector3 newPosition = _shadowMask.localPosition;
        if (directionTotakeFrom == Vector2.left)
        {
            if (_lastTransmutationDirection != DIR.LEFT && _lastTransmutationDirection!=DIR.NONE) return;
            if (_shadowMask.localScale.x < 0) return;
            _lastTransmutationDirection = DIR.LEFT;
            _transmutateValue = Time.deltaTime * 2f;
            newScale.x -= _transmutateValue;
            _shadowMask.localScale = newScale;
            newPosition.x += _transmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position.x, _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x, _spriteMask.bounds.min.x)
                + math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _spriteMask.bounds.max.x);//math.remap( ,_spriteMask.bounds.max.x) //_totalShadowbarValue * _curve.Evaluate(scaleBound+ _transmutateValue) - _totalShadowbarValue * _curve.Evaluate(scaleBound);
            if (_spriteMask.bounds.min.x >= _segments[_segmentsTakenPerSide[((int)DIR.LEFT)]+1].position.x)
            {
                _shadowSegmentsList.Add(DIR.LEFT);
                _segmentsTakenPerSide[((int)DIR.LEFT)]++;
                _segmentsTaken++;
            }
        }
        if (directionTotakeFrom == Vector2.right)
        {
            if (_lastTransmutationDirection != DIR.RIGHT && _lastTransmutationDirection != DIR.NONE) return;
            if (_shadowMask.localScale.x < 0) return;
            _lastTransmutationDirection = DIR.RIGHT;
            _transmutateValue = Time.deltaTime * 2f;
            newScale.x -= _transmutateValue;
            _shadowMask.localScale = newScale;
            newPosition.x -= _transmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position.x, _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x, _spriteMask.bounds.min.x)
    + math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _spriteMask.bounds.max.x);
            if (_spriteMask.bounds.max.x <= _segments[_segments.Count - 2 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x)
            {
                _shadowSegmentsList.Add(DIR.RIGHT);
                _segmentsTakenPerSide[((int)DIR.RIGHT)]++;
                _segmentsTaken++;
            }
        }
        _transmutateValue = 0;
    }
    private void SetValueForShadowBar()
    {
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position.x, _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x, _spriteMask.bounds.min.x)
+ math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _spriteMask.bounds.max.x);
    }
    public void RevertNonSegmentShadowbar()
    {
        StartCoroutine(RevertNonBarTransmutation());
    }
    public void RevertTransmutationFromPlacedShadow(float _shadowBarCost)
    {
        StartCoroutine(RevertLastBarTransmutation());
    }
    public void RevertTransmutation()
    {
        if (_valueForShadowPlacing+_placedShadows.Count > _segmentsTaken)
        {
            StartCoroutine(RevertNonBarTransmutation());
        }
        else
        {
            if(_segmentsTaken > _placedShadows.Count) StartCoroutine(RevertLastBarTransmutation());

        }
    }
    public void RevertAllTran()
    {
        StartCoroutine(RevertAllTransmutation());
    }
    public IEnumerator RevertAllTransmutation()
    {
        for(int i= _placedShadows.Count-1; i>=0;i--)
        {
            _placedShadows[i].OnLeftParentShadow -= RemovePlacedShadow;
            //RevertTransmutationFromPlacedShadow(_placedShadows[_placedShadows.Count - 1].ShadowBarCost);
            Destroy(_placedShadows[i ].gameObject);
            _placedShadows.RemoveAt(i );
        }
        int num = _segmentsTaken;
        for (int i = 0; i < num; i++)
        {
            yield return RevertLastBarTransmutation();
        }

    }
    IEnumerator RevertLastBarTransmutation()
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        bool isClear = false;
        DIR tmp = _shadowSegmentsList[_shadowSegmentsList.Count - 1];
        _shadowSegmentsList.RemoveAt(_shadowSegmentsList.Count - 1);
        while(!isClear)
        {
            switch(tmp)
            {
                case DIR.LEFT: 
                    {
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.x += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.min.x <= _segments[_segmentsTakenPerSide[((int)DIR.LEFT)]-1].position.x)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)] - 1].position).x- _shadow.InverseTransformPoint(_spriteMask.bounds.min).x ;
                            newScale.x -= diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x += diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            _segmentsTakenPerSide[((int)DIR.LEFT)]--;
                            _segmentsTaken--;
                        }
                        break;
                    }
                    case DIR.RIGHT:
                    {
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.x += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.x += _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.max.x >= _segments[_segments.Count - 1 - _segmentsTakenPerSide[ ((int)DIR.RIGHT)]+1].position.x)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.max).x- _shadow.InverseTransformPoint(_segments[_segments.Count - 1 -_segmentsTakenPerSide[((int)DIR.RIGHT)] + 1].position).x;
                            newScale.x -= diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            _segmentsTakenPerSide[((int)DIR.RIGHT)]--;
                            _segmentsTaken--;
                        }
                        break;
                    }
                case DIR.UP: { break; }
                    case DIR.DOWN: { break; }
            }
            SetValueForShadowBar();
            yield return null;
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;

    }
    IEnumerator RevertNonBarTransmutation()
    {
        Vector2  newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        bool isAllClear = false;
        while (!isAllClear)
        {
            isAllClear = true;
            for (int i = 0; i < 4; i++)
            {
                newScale = _shadowMask.localScale;
                newPosition = _shadowMask.localPosition;
                switch ((DIR)i)
                {
                    case DIR.UP:
                        {
                            break;
                        }
                    case DIR.DOWN: { break; }
                    case DIR.LEFT:
                        {
                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.min).x - _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position).x;
                            if (math.abs(diff - 0) < 0.001f) continue;
                            isAllClear = false;
                            newScale.x += diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.RIGHT:
                        {
                            float diff = _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position).x- _shadow.InverseTransformPoint(_spriteMask.bounds.max).x ;
                            if (math.abs(diff - 0) < 0.001f) continue;
                            isAllClear = false;
                            newScale.x += diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x += diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }


                }
                SetValueForShadowBar();
                yield return null;
            }
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;
        _lastTransmutationDirection = DIR.NONE;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,_distanceToResetShadow);
    }
    private void OnValidate()
    {
        if(_resetShadowCollider) _resetShadowCollider.radius = _distanceToResetShadow;

    }
}
