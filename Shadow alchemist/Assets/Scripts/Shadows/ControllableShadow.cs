using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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
    [Tooltip("Set curve value at begining and end to 0. Set its value at lowest/highest point. Create polygon collider as child of SHAdow Controller.Check its points. "), SerializeField] bool _ajustColliderByCurve;
    [SerializeField] protected float _transmutationSpeed=2f;
    [SerializeField] AnimationCurve _curve;
   // [SerializeField] bool _isTriangle;
    [SerializeField] protected bool _isHorizontal = true;
    [SerializeField] protected Transform _shadowMask;
    [SerializeField] protected float scaleToPoSrate = 2f;
    [SerializeField] protected float _totalShadowbarValue;
    [SerializeField] protected float _distanceToResetShadow;
    [SerializeField] protected CircleCollider2D  _resetShadowCollider;
    [Header("Borders"),SerializeField] protected Transform _lefBorder;
    [SerializeField] protected Transform _rightBorder;
    [SerializeField] protected Transform _upperBorder;
    [SerializeField] protected Transform _lowerBorder;
    [SerializeField] protected SpriteMask _spriteMask;
    [Header("Segments"),SerializeField] protected List<Transform> _segments;
    protected List<PlacableShadow> _placedShadows= new List<PlacableShadow>();
    protected float _transmutateValue = 0;
    protected float _revertTransmutateValue = 0;
    protected float _valueForShadowPlacing;
    protected int _segmentsTaken=0;
    protected int[] _segmentsTakenPerSide= new int[4] {0,0,0,0 };
    protected Vector2 _originalPosition;
    protected Coroutine _revertMoveCor;
    protected List<DIR> _shadowSegmentsList= new List<DIR>();
    protected DIR _lastTransmutationDirection =DIR.NONE;
    protected bool _isReverting = false;
    protected Vector2 _shadowShift;
   // private Vector2[] _points;
   // private Vector2[] _originalPositions;

    //private float _curveShift = 0;
    protected void Start()
    {

        //if (_ajustColliderByCurve)
        //{
        //    _points = ((PolygonCollider2D)_shadowCollider).points;
        //     _originalPositions = ((PolygonCollider2D)_shadowCollider).points;

        //}
        //if (_ajustColliderByCurve) _originalPositions = ((PolygonCollider2D)_shadowCollider).points;
        _originalPosition = _shadow.position;
    }
    #region ShadowMove
    public virtual void MoveShadow(float moveSpeed,Vector2 direction)
    {
        if(_revertMoveCor!=null)
        {
            StopCoroutine(_revertMoveCor);
            _revertMoveCor = null;
        }

        
        if (_isHorizontal)
        {
            if (direction == Vector2.right)
            {
                if (_shadow.transform.position.x - _shadow.transform.localScale.x / 2 < _lefBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);

                //    if (_ajustColliderByCurve)
                //    {
                //        for (int i = 0; i < _points.Length; i++)
                //        {
                //            _points[i] += direction * moveSpeed * Time.deltaTime;
                //            _originalPositions[i] += direction * moveSpeed * Time.deltaTime;
                //        }
                //         ((PolygonCollider2D)_shadowCollider).points = _points;
                //    }
                //    _curveShift += direction.x * moveSpeed * Time.deltaTime;
                }
            }
            if (direction == Vector2.left)
            {
                if (_shadow.transform.position.x + _shadow.transform.localScale.x / 2 > _rightBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    //    if (_ajustColliderByCurve)
                    //    {
                    //        for (int i = 0; i < _points.Length; i++)
                    //        {

                    //            _points[i] += direction * moveSpeed * Time.deltaTime;
                    //            _originalPositions[i] += direction * moveSpeed * Time.deltaTime;
                    //        }
                    //         ((PolygonCollider2D)_shadowCollider).points = _points;
                    //    }
                    //    _curveShift += direction.x * moveSpeed * Time.deltaTime;
                    //}
                }
            }
        }
        else
        {
            if (direction == Vector2.up)
            {
                if (_shadow.transform.position.y - _shadow.transform.localScale.y / 2 < _upperBorder.position.y)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
            }
            if (direction == Vector2.down)
            {
                if (_shadow.transform.position.y + _shadow.transform.localScale.y / 2 > _lowerBorder.position.y)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                }
            }
        }
        _shadowShift = direction * moveSpeed * Time.deltaTime;

    }
    public virtual void RevertMove()
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
        RevertTransmutationFromPlacedShadow(shadow.ShadowBarCost);
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

    public virtual void Transmutate(Vector2 directionTotakeFrom)
    {

        Vector2 newScale = _shadowMask.localScale;
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
                SetValueForShadowBar();// _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.UP)]].position.y, _segments[_segmentsTakenPerSide[((int)DIR.UP)] + 1].position.y, _spriteMask.bounds.min.y)
        //+ math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.DOWN)]].position.y, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.DOWN)]].position.y, _spriteMask.bounds.max.y);
                if (_spriteMask.bounds.max.y <= _segments[_segmentsTakenPerSide[((int)DIR.UP)]+1].position.y)
                {
                    _shadowSegmentsList.Add(DIR.UP);
                    _segmentsTakenPerSide[((int)DIR.UP)]++;
                    _segmentsTaken++;
                }
            }
        }

        _transmutateValue = 0;
        //if(_ajustColliderByCurve)
        //{
        //    _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        //    _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        //    _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        //    _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        //    _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        //    _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        //    if (directionTotakeFrom == Vector2.left)
        //    {

                
        //        for (int i = 2; i < _points.Length-2; i++)
        //        {
        //            //_points[i].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        //            if (_points[i - 1].x > _points[i].x) _points[i].x = _points[i - 1].x;
        //            _points[i].y = _curve.Evaluate(_points[i].x-_curveShift);
        //        }
        //    }
        //    else if(directionTotakeFrom == Vector2.right)
        //    {

                
        //        for (int i = _points.Length - 3; i >= 2; i--)
        //        {
        //            //_points[i].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        //            if (_points[i + 1].x < _points[i].x) _points[i].x = _points[i + 1].x;
        //            _points[i].y = _curve.Evaluate(_points[i].x- _curveShift);
        //        }
        //    }
        //     ((PolygonCollider2D)_shadowCollider).points = _points;


        //}
    }
    protected void SetValueForShadowBar()
    {
        if (_isHorizontal)
        {
            _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position.x, _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position.x, _spriteMask.bounds.min.x)
            + math.unlerp(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position.x, _spriteMask.bounds.max.x);
        }
        else
        {
            _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count + math.unlerp(_segments[_segmentsTakenPerSide[((int)DIR.UP)]].position.y, _segments[ _segmentsTakenPerSide[((int)DIR.UP)]+1 ].position.y, _spriteMask.bounds.max.y)
            + math.unlerp(_segments[(_segments.Count - 1) - _segmentsTakenPerSide[((int)DIR.DOWN)]].position.y, _segments[(_segments.Count - 2) - _segmentsTakenPerSide[((int)DIR.DOWN)]].position.y, _spriteMask.bounds.min.y);
        }
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
    virtual protected IEnumerator RevertLastBarTransmutation()
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
                        if (!_isHorizontal) break;
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
                        //if (_ajustColliderByCurve)
                        //{
                        //    _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                        //    _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                        //    _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                        //    _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
                        //    _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                        //    _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;

                        //    for (int j = 2; j < _points.Length - 2; j++)
                        //    {
                        //        if (_points[j].x > transform.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)] + 1].position).x) continue;
                        //        //_points[i].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                        //        if (_points[j - 1].x < _points[j].x) _points[j].x = _points[j - 1].x;
                        //        if (transform.InverseTransformPoint(_spriteMask.bounds.max).x > _originalPositions[j].x && transform.InverseTransformPoint(_spriteMask.bounds.min).x < _originalPositions[j].x)
                        //        {
                        //            _points[j] = _originalPositions[j];
                        //            continue;
                        //        }
                        //        _points[j].y = _curve.Evaluate(_points[j].x- _curveShift);
                        //    }

                        //    ((PolygonCollider2D)_shadowCollider).points = _points;


                        //}
                        break;
                    }
                case DIR.RIGHT:
                    {
                        if (!_isHorizontal) break;
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.x += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.x += _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.max.x >= _segments[_segments.Count - 1 - _segmentsTakenPerSide[ ((int)DIR.RIGHT)]+1].position.x)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.max).x- _shadow.InverseTransformPoint(_segments[_segments.Count - 1 -_segmentsTakenPerSide[((int)DIR.RIGHT)] + 1].position).x;
                            //float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.max).x -_shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position).x 
                            newScale.x -= diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            _segmentsTakenPerSide[((int)DIR.RIGHT)]--;
                            _segmentsTaken--;
                        }
                        //if (_ajustColliderByCurve)
                        //{
                        //    _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                        //    _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                        //    _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                        //    _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
                        //    _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                        //    _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;


                        //    for (int j = _points.Length - 3; j >= 2; j--)
                        //    {
                        //        //_points[i].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                        //        if (_points[j].x < transform.InverseTransformPoint(_segments[_segments.Count - 2 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position).x) continue;
                        //        if (_points[j + 1].x > _points[j].x) _points[j].x = _points[j + 1].x;
                        //        if (transform.InverseTransformPoint(_spriteMask.bounds.max).x > _originalPositions[j].x && transform.InverseTransformPoint(_spriteMask.bounds.min).x < _originalPositions[j].x)
                        //        {
                        //            _points[j] = _originalPositions[j];
                        //            continue;
                        //        }
                        //        _points[j].y = _curve.Evaluate(_points[j].x - _curveShift);
                        //    }


                        //     ((PolygonCollider2D)_shadowCollider).points = _points;


                        //}
                        break;
                    }
                case DIR.UP: 
                    {
                        if (_isHorizontal) break;
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.y += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.y += _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.max.y >= _segments[ _segmentsTakenPerSide[((int)DIR.UP)] - 1].position.y)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.max).y - _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.UP)] - 1].position).y;
                            newScale.y -= diff;
                            _shadowMask.localScale = newScale;
                            newPosition.y -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            _segmentsTakenPerSide[((int)DIR.UP)]--;
                            _segmentsTaken--;
                        }
                        break; 
                    }
                case DIR.DOWN:
                    {
                        if (_isHorizontal) break;
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.y += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.y -= _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.min.y <= _segments[_segments.Count-1- _segmentsTakenPerSide[((int)DIR.DOWN)]+1].position.y)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_segments[_segments.Count - 1-_segmentsTakenPerSide[((int)DIR.DOWN)]+1].position).y - _shadow.InverseTransformPoint(_spriteMask.bounds.min).y;
                            newScale.y -= diff;
                            _shadowMask.localScale = newScale;
                            newPosition.y += diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            _segmentsTakenPerSide[((int)DIR.DOWN)]--;
                            _segmentsTaken--;
                        }
                        break; 
                    }
            }
            SetValueForShadowBar();
            yield return null;
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;

    }
    virtual protected IEnumerator RevertNonBarTransmutation()
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
                            if (_isHorizontal) break;
                            float diff = _shadow.InverseTransformPoint(_segments[ _segmentsTakenPerSide[((int)DIR.UP)]].position).y - _shadow.InverseTransformPoint(_spriteMask.bounds.max).y;
                            Logger.Log(diff);
                            if (math.abs(diff - 0) < 0.001f || diff<0) continue;
                            isAllClear = false;
                            newScale.y += diff;
                            _shadowMask.localScale = newScale;
                            newPosition.y += diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.DOWN: 
                        {
                            if (_isHorizontal) break;
                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.min).y - _shadow.InverseTransformPoint(_segments[(_segments.Count - 1) - _segmentsTakenPerSide[((int)DIR.DOWN)]].position).y;
                            if (math.abs(diff - 0) < 0.001f) continue;
                            isAllClear = false;
                            newScale.y += diff;
                            _shadowMask.localScale = newScale;
                            newPosition.y -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break; 
                        }
                    case DIR.LEFT:
                        {
                            if (!_isHorizontal) break;
                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.min).x - _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]].position).x;
                            if (math.abs(diff - 0) < 0.001f) continue;
                            isAllClear = false;
                            newScale.x += diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;

                            //if (_ajustColliderByCurve)
                            //{
                            //    _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                            //    _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                            //    _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                            //    _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
                            //    _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                            //    _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;



                            //    for (int j = 2; j < _points.Length - 2; j++)
                            //    {
                            //        //_points[i].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                            //        if (_points[j].x > transform.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)]+1].position).x) continue;
                            //        if (_points[j - 1].x < _points[j].x) _points[j].x = _points[j - 1].x;
                            //        if (transform.InverseTransformPoint(_spriteMask.bounds.max).x > _originalPositions[j].x && transform.InverseTransformPoint(_spriteMask.bounds.min).x < _originalPositions[j].x)
                            //        {
                            //            _points[j] = _originalPositions[j];
                            //            continue;
                            //        }
                            //        _points[j].y = _curve.Evaluate(_points[j].x - _curveShift);
                            //    }

                            //    ((PolygonCollider2D)_shadowCollider).points = _points;


                            //}
                            break;

                        }
                    case DIR.RIGHT:
                        {
                            if (!_isHorizontal) break;
                            float diff = _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position).x- _shadow.InverseTransformPoint(_spriteMask.bounds.max).x ;
                            if (math.abs(diff - 0) < 0.001f) continue;
                            isAllClear = false;
                            newScale.x += diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x += diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            //if (_ajustColliderByCurve)
                            //{
                            //    _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                            //    _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                            //    _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                            //    _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
                            //    _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
                            //    _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;





                            //    for (int j = _points.Length - 3; j >= 2; j--)
                            //    {
                            //        if (_points[j].x < transform.InverseTransformPoint(_segments[_segments.Count - 2 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position).x) continue;
                            //        //_points[i].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
                            //        if (_points[j + 1].x > _points[j].x) _points[j].x = _points[j + 1].x;
                            //        if (transform.InverseTransformPoint(_spriteMask.bounds.max).x > _originalPositions[j].x && transform.InverseTransformPoint(_spriteMask.bounds.min).x< _originalPositions[j].x)
                            //        {
                            //            _points[j] = _originalPositions[j];
                            //            continue;
                            //        }
                            //        _points[j].y = _curve.Evaluate(_points[j].x - _curveShift);
                            //    }

                            //((PolygonCollider2D)_shadowCollider).points = _points;


                            //}
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
