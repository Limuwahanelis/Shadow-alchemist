using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class ControllableShadow : MonoBehaviour
{
    public enum DIR
    {
        NONE=-1,LEFT,UP,RIGHT,DOWN
    }
    public bool IsHorizontal => _isHorizontal;
    public Collider2D ShadowCollider => _shadowCollider;
    public float TakenShadowBarValueFromTransmutation=>_valueForShadowPlacing;
    public Bounds ShadowBounds => _spriteMask.bounds;
    [SerializeField] protected Transform _shadow;
    [Header("Shadow"),SerializeField] protected Collider2D _shadowCollider;
    [SerializeField] protected float _transmutationSpeed=2f;
    [SerializeField] protected float _revertTransmutationSpeed=8f;
    [SerializeField] protected bool _isHorizontal = true;
    [SerializeField] protected Transform _shadowMask;
    [SerializeField] protected float scaleToPoSrate = 2f;
    [SerializeField] protected float _distanceToResetShadow;
    [SerializeField] protected CircleCollider2D  _resetShadowCollider;
    [Header("Borders"),SerializeField, ConditionalField(nameof(_isHorizontal))] protected ShadowRangeIndicator _leftRangeIndicator;
    [SerializeField, ConditionalField(nameof(_isHorizontal))] protected ShadowRangeIndicator _rightRangeIndicator;
    [SerializeField, ConditionalField(nameof(_isHorizontal), inverse: true)] protected ShadowRangeIndicator _upperRangeIndicator;
    [SerializeField, ConditionalField(nameof(_isHorizontal), inverse: true)] protected ShadowRangeIndicator _lowerRangeIndicator;
    [SerializeField] protected SpriteMask _spriteMask;
    [Header("Segments"),SerializeField] protected List<Transform> _segments;
    protected DIR _lastTransmutationDirection =DIR.NONE;
    protected List<PlacableShadow> _placedShadows= new List<PlacableShadow>();
    protected Vector2 _originalPosition;
    protected Vector2 _shadowShift;
    protected Coroutine _revertMoveCor;
    protected List<DIR> _shadowSegmentsList= new List<DIR>();
    protected float _transmutateValue = 0;
    protected float _revertTransmutateValue = 0;
    protected float _valueForShadowPlacing;
    protected int[] _segmentsTakenPerSide= new int[4] {0,0,0,0 };
    protected int _segmentsTaken=0;
    protected bool _isReverting = false;
    protected bool _isRevertingMove = false;
    
    protected virtual void Start()
    {
        _originalPosition = _shadow.position;
    }
    #region ShadowMove
    public virtual void MoveShadow(float moveSpeed,Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            if (_revertMoveCor != null)
            {
                StopCoroutine(_revertMoveCor);
                _isRevertingMove = false;
                _revertMoveCor = null;
            }
        }
        if (_isHorizontal)
        {
            
            if (direction == Vector2.right)
            {
                if (_shadow.transform.position.x < _rightRangeIndicator.transform.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowMask.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.x += direction.x * moveSpeed * Time.deltaTime;
                }

            }
            if (direction == Vector2.left)
            {
                if (_shadow.transform.position.x > _leftRangeIndicator.transform.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowMask.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.x += direction.x * moveSpeed * Time.deltaTime;
                }
            }
        }
        else
        {

            if (direction == Vector2.up)
            {
                if (_shadow.transform.position.y < _upperRangeIndicator.transform.position.y)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowMask.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.y += direction.y * moveSpeed * Time.deltaTime;
                }
            }
            if (direction == Vector2.down)
            {
                if (_shadow.transform.position.y > _lowerRangeIndicator.transform.position.y)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowMask.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.y += direction.y * moveSpeed * Time.deltaTime;
                }
            }

        }

    }
    public virtual void RevertMove()
    {
        if (_isReverting || _isRevertingMove) return;
        _revertMoveCor = StartCoroutine(RevertShadowMove());
    }
    protected virtual void RevertShadowMoveStep()
    {
        Vector3 newPos = Vector2.MoveTowards(_shadow.position, _originalPosition, 3f * Time.deltaTime);
        Vector2 tmp = (newPos - _shadow.position);
        _shadowShift += tmp;
        _shadowMask.position += new Vector3(tmp.x, tmp.y, 0);
        _shadow.position = newPos;
    }
    protected IEnumerator RevertShadowMove()
    {
        _isRevertingMove = true;
        while (Vector2.Distance(_shadow.position, _originalPosition) > 0.0001)
        {
            RevertShadowMoveStep();
            yield return null;
        }
        _isRevertingMove = false;
    }
    public void SetMoveRangeVisibility(bool isVisiblie)
    {
        if (_isHorizontal)
        {
            _leftRangeIndicator.SetVisibility(isVisiblie);
            _rightRangeIndicator.SetVisibility(isVisiblie);
        }
        else
        {
            _lowerRangeIndicator.SetVisibility(isVisiblie);
            _upperRangeIndicator.SetVisibility(isVisiblie);
        }
    }
    #endregion
    #region ShadowPlacing
    public void PlaceNewShadow(PlacableShadow newShadow)
    {
        _placedShadows.Add(newShadow);
        _valueForShadowPlacing-=newShadow.ShadowBarCost;
    }
    public void RemovePlacedShadow(PlacableShadow shadow)
    {
        _placedShadows.Remove(shadow);
        _valueForShadowPlacing += shadow.ShadowBarCost;
        _placedShadows.Remove(shadow);
        Destroy(shadow.gameObject);
    }
    public void RemoveRecentShadow()
    {
        if (_placedShadows.Count == 0) return;
        Destroy(_placedShadows[_placedShadows.Count - 1].gameObject);
        
    }
    #endregion
    #region ShadowTransmutation
    public virtual void Transmutate(Vector2 directionTotakeFrom)
    {
        if (_isReverting) return;
        Vector2 newScale = _shadowMask.lossyScale;
        Vector3 newPosition = _shadowMask.localPosition;
        if (_isHorizontal)
        {
            if (directionTotakeFrom == Vector2.left)
            {
                if (_lastTransmutationDirection != DIR.LEFT && _lastTransmutationDirection != DIR.NONE) return;
                if (_shadowMask.localScale.x < 0) return;
                _lastTransmutationDirection = DIR.LEFT;
                _transmutateValue = Time.deltaTime * _transmutationSpeed;
                newScale.x -= _transmutateValue;
                 _shadowMask.localScale = newScale;
                newPosition.x += _transmutateValue / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position.x, _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x, _spriteMask.bounds.min.x)
                    + math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _spriteMask.bounds.max.x);
                if (_spriteMask.bounds.min.x >= _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x)
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
                _transmutateValue = Time.deltaTime * _transmutationSpeed;
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
        }
        else
        {
            if (directionTotakeFrom == Vector2.down)
            {
                
                if (_lastTransmutationDirection != DIR.DOWN && _lastTransmutationDirection != DIR.NONE) return;
                if (_shadowMask.localScale.y < 0) return;
                _lastTransmutationDirection = DIR.DOWN;
                _transmutateValue = Time.deltaTime * _transmutationSpeed;
                newScale.y -= _transmutateValue;
                _shadowMask.localScale = newScale;
                newPosition.y += _transmutateValue / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                SetValueForShadowBar();
                if (_spriteMask.bounds.min.y >= _segments[(_segments.Count - 2)-_segmentsTakenPerSide[((int)DIR.DOWN)] ].position.y)
                {
                    _shadowSegmentsList.Add(DIR.DOWN);
                    _segmentsTakenPerSide[((int)DIR.DOWN)]++;
                    _segmentsTaken++;
                    Debug.Log("ADasdd");
                }
            }
            if (directionTotakeFrom == Vector2.up)
            {
                if (_lastTransmutationDirection != DIR.UP && _lastTransmutationDirection != DIR.NONE) return;
                if (_shadowMask.localScale.y < 0) return;
                _lastTransmutationDirection = DIR.UP;   
                _transmutateValue = Time.deltaTime * _transmutationSpeed;
                newScale.y -= _transmutateValue;
                _shadowMask.localScale = newScale;
                newPosition.y -= _transmutateValue / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                SetValueForShadowBar();
                if (_spriteMask.bounds.max.y <= _segments[_segmentsTakenPerSide[((int)DIR.UP)]+1].position.y)
                {
                    _shadowSegmentsList.Add(DIR.UP);
                    _segmentsTakenPerSide[((int)DIR.UP)]++;
                    _segmentsTaken++;
                }
            }
        }

        _transmutateValue = 0;
    }

    public void RevertNonSegmentShadowbar()
    {
        if(_isReverting) return;
        StartCoroutine(RevertNonBarTransmutation());
    }

    public void RevertTransmutationFromPlacedShadow(float _shadowBarCost)
    {
        if (_isReverting) return;
        StartCoroutine(RevertLastBarTransmutation());
    }
    public void RevertTransmutation()
    {
        if (_isReverting) return;
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
        if (_isReverting) return;
        StartCoroutine(RevertAllTransmutation());
    }
    public IEnumerator RevertAllTransmutation()
    {
        for(int i= _placedShadows.Count-1; i>=0;i--)
        {
            Destroy(_placedShadows[i ].gameObject);
            //_placedShadows.RemoveAt(i );
        }
        int num = _segmentsTaken;
        for (int i = 0; i < num; i++)
        {
            yield return RevertLastBarTransmutation();
        }

    }
    virtual protected IEnumerator RevertLastBarTransmutation()
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        bool isClear = false;
        DIR tmp = _shadowSegmentsList[_shadowSegmentsList.Count - 1];
        _shadowSegmentsList.RemoveAt(_shadowSegmentsList.Count - 1);
        _isReverting = true;
        while (!isClear)
        {

            _revertTransmutateValue = Time.deltaTime * _revertTransmutationSpeed;
            isClear=RevertSegmentStep(tmp);
            SetValueForShadowBar();
            yield return null;
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;
        _isReverting = false;
    }
    virtual protected IEnumerator RevertNonBarTransmutation()
    {
        bool isAllClear = false;
        
        _isReverting = true;
        while (!isAllClear)
        {
            _revertTransmutateValue = Time.deltaTime * _revertTransmutationSpeed;
            isAllClear = RevertNonSegmentStep(_lastTransmutationDirection);
            SetValueForShadowBar();
            yield return null;
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;
        _lastTransmutationDirection = DIR.NONE;
        _isReverting = false;
    }
    protected virtual bool RevertSegmentStep(DIR revertDirection)
    {
        bool isClear;
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        switch (revertDirection)
        {
            case DIR.LEFT:
                {
                    if (!_isHorizontal) break;
                    isClear = RevertHorizontalTransmutation(DIR.LEFT, true);
                    if (isClear)
                    {
                        _segmentsTakenPerSide[((int)DIR.LEFT)]--;
                        _segmentsTaken--;
                        return true;
                    }
                    return false;
                }
            case DIR.RIGHT:
                {
                    if (!_isHorizontal) break;
                    isClear = RevertHorizontalTransmutation(DIR.RIGHT, true);
                    if (isClear)
                    {
                        _segmentsTakenPerSide[((int)DIR.RIGHT)]--;
                        _segmentsTaken--;
                        return true;
                    }
                    return false;
                }
            case DIR.UP:
                {
                    if (_isHorizontal) break;
                    isClear = RevertVerticalTransmutation(DIR.UP, true);
                    if(isClear)
                    {
                        _segmentsTakenPerSide[((int)DIR.UP)]--;
                        _segmentsTaken--;
                        return true;
                    }
                    return false;
                }
            case DIR.DOWN:
                {
                    if (_isHorizontal) break;
                    isClear = RevertVerticalTransmutation(DIR.DOWN, true);
                    if(isClear)
                    {
                        _segmentsTakenPerSide[((int)DIR.DOWN)]--;
                        _segmentsTaken--;
                        return true;
                    }
                    return false;
                }
            case DIR.NONE: return true;
        }
        Debug.LogError($"Somthing happend here {revertDirection}");
        return false;
    }
    protected virtual bool RevertNonSegmentStep(DIR revertDirection)
    {
        switch (revertDirection)
        {
            case DIR.UP:
                {
                    if (_isHorizontal) break;
                    return RevertVerticalTransmutation(DIR.UP,false);
                }
            case DIR.DOWN:
                {
                    if (_isHorizontal) break;
                    return RevertVerticalTransmutation(DIR.DOWN,false);
                }
            case DIR.LEFT:
                {
                    if (!_isHorizontal) break;
                    return RevertHorizontalTransmutation(DIR.LEFT, false);

                }
            case DIR.RIGHT:
                {
                    if (!_isHorizontal) break;
                    return RevertHorizontalTransmutation(DIR.RIGHT, false);
                }
            case DIR.NONE: return true;

        }
        Logger.Error("Somthing happend here");
        return false;
    }
    #endregion
    protected void SetValueForShadowBar()
    {
        if (_isHorizontal)
        {
            _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position.x, _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x, _spriteMask.bounds.min.x)
            + math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _spriteMask.bounds.max.x);
        }
        else
        {
            _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.UP)]].position.y, _segments[_segmentsTakenPerSide[((int)DIR.UP)] + 1].position.y, _spriteMask.bounds.max.y)
            + math.unlerp(_segments[(_segments.Count - 1) - _segmentsTakenPerSide[((int)DIR.DOWN)]].position.y, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.DOWN)]].position.y, _spriteMask.bounds.min.y);
        }
    }
    /// <summary>
    /// Returns true if shadow mask horizontal max bound or min bound reached last segment 
    /// </summary>
    /// <param name="revertDirection"></param>
    /// <returns></returns>
    protected bool RevertHorizontalTransmutation(DIR revertDirection, bool revertSegment)
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        newScale.x += _revertTransmutateValue;
        _shadowMask.localScale = newScale;
        if (revertDirection == DIR.RIGHT)
        {
            newPosition.x += _revertTransmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            if (_spriteMask.bounds.max.x >= _segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)] + (revertSegment ? 1 : 0)].position.x)
            {
                float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.max).x - _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)] + (revertSegment ? 1 : 0)].position).x;
                newScale.x -= diff;
                _shadowMask.localScale = newScale;
                newPosition.x -= diff / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                return true;
            }
            else return false;

        }
        else
        {
            newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            if (_spriteMask.bounds.min.x <= _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] - (revertSegment ? 1 : 0)].position.x)
            {
                float diff = _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)] - (revertSegment ? 1 : 0)].position).x - _shadow.InverseTransformPoint(_spriteMask.bounds.min).x;
                newScale.x -= diff;
                _shadowMask.localScale = newScale;
                newPosition.x += diff / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                return true;
            }
            else return false;
        }
    }
    /// <summary>
    /// Returns true if shadow mask vertical max bound or min bound reached last segment 
    /// </summary>
    /// <param name="revertDirection"></param>
    /// <returns></returns>
    protected bool RevertVerticalTransmutation(DIR revertDirection, bool revertSegment)
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        newScale.y += _revertTransmutateValue;
        _shadowMask.localScale = newScale;
        if (revertDirection == DIR.DOWN)
        {
            newPosition.y -= _revertTransmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            if (_spriteMask.bounds.min.y < _segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.DOWN)] + (revertSegment ? 1 : 0)].position.y)
            {
                float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.min).y - _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.DOWN)] + (revertSegment ? 1 : 0)].position).y;
                newScale.y += diff;
                _shadowMask.localScale = newScale;
                newPosition.y -= diff / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            newPosition.y += _revertTransmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            if (_spriteMask.bounds.max.y > _segments[_segmentsTakenPerSide[((int)DIR.UP)] - (revertSegment ? 1 : 0)].position.y)
            {
                float diff = _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.UP)] - (revertSegment ? 1 : 0)].position).y - _shadow.InverseTransformPoint(_spriteMask.bounds.max).y;
                newScale.y += diff;
                _shadowMask.localScale = newScale;
                newPosition.y += diff / scaleToPoSrate;
                _shadowMask.localPosition = newPosition;
                return true;
            }
            else return false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_resetShadowCollider.transform.position,_distanceToResetShadow);
    }
    private void OnValidate()
    {
        if(_resetShadowCollider) _resetShadowCollider.radius = _distanceToResetShadow;

    }
}
