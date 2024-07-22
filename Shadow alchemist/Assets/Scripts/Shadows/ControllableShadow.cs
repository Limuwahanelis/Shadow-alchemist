using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ControllableShadow : MonoBehaviour, IMovableShadow,ITransmutableSadow
{
    public enum DIR
    {
        LEFT,UP,RIGHT,DOWN
    }
    public Collider2D ShadowCollider => _shadowCollider;
    public float TakenShadowBarValueFromTransmutation=>_valueForShadowPlacing;
    [SerializeField] Transform _lefBorder;
    [SerializeField] Transform _rightBorder;
    [SerializeField] Transform _shadow;
    [SerializeField] Transform _shadowMask;
    [SerializeField] Collider2D _shadowCollider;
    [SerializeField] float scaleToPoSrate = 2f;
    [SerializeField] AnimationCurve _curve;
    [SerializeField] float _totalShadowbarValue;
    [SerializeField] float _revertTransmutationError;
    private List<PlacableShadow> _placedShadows= new List<PlacableShadow>();
    private float[] _transmutatedShadow = new float[4]; // stores how much scale was reduced from orignal
    private float[] _transmutatedShadowCutOff= new float[4]; // stores how much scale is in placed shadows
    private float _transmutateValue = 0;
    private float _revertTransmutateValue = 0;
    private Vector2 _originalPosition;
    private Coroutine _revertMoveCor;
    public float _valueForShadowPlacing;
    public float restoredBarVal;
    [SerializeField] SpriteMask _spriteMask;
    [SerializeField] bool _isHorizontal;
    private Vector3 _originalMinBound;
    private Vector3 _originalMaxBound;
    private float _lastOverShootValue;
    float _revertbarOverflow;
    private void Start()
    {
        _originalPosition = _shadow.position;
        _originalMinBound = _spriteMask.bounds.min;
        _originalMaxBound = _spriteMask.bounds.max;
        Logger.Log(_spriteMask.bounds.max);
        Logger.Log(_spriteMask.bounds.min);
        Logger.Log(_spriteMask.bounds.extents);
        Logger.Log(_spriteMask.bounds.center);
        Logger.Log(_spriteMask.bounds.size);
    }
    #region ShadowMove
    public void MoveShadow(float moveSpeed,Vector2 direction)
    {
        if(_revertMoveCor!=null)
        {
            StopCoroutine(_revertMoveCor);
            _revertMoveCor = null;
        }
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
    public void RevertTransmutationFromPlacedShadow(float _shadowBarCost)
    {
        StartCoroutine(RevertAllTransmutationFromPlacedShadow2(_shadowBarCost));
    }
    public void RevertTransmutation()
    {
        StartCoroutine(RevertAllTransmutation2());
    }

    public void Transmutate()
    {
        
    }

    public void Transmutate(Vector2 directionTotakeFrom)
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector3 newPosition = _shadowMask.localPosition;
        float scaleBound;
        if (directionTotakeFrom == Vector2.left)
        {
            if (_shadowMask.localScale.x < 0) return;

            _transmutateValue = Time.deltaTime * 2f;
            newScale.x -= _transmutateValue;
            _shadowMask.localScale = newScale;
            newPosition.x += _transmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            scaleBound = math.remap(_originalMinBound.x, _originalMaxBound.x, 0, 1, _spriteMask.bounds.min.x);
            //_transmutatedShadow[((int)DIR.LEFT)] += _transmutateValue;
            _valueForShadowPlacing += _totalShadowbarValue * _curve.Evaluate(scaleBound+ _transmutateValue) - _totalShadowbarValue * _curve.Evaluate(scaleBound);
        }
        if (directionTotakeFrom == Vector2.right)
        {
            if (_shadowMask.localScale.x < 0) return;
            _transmutateValue = Time.deltaTime * 2f;
            newScale.x -= _transmutateValue;
            _shadowMask.localScale = newScale;
            newPosition.x -= _transmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            _transmutatedShadow[((int)DIR.RIGHT)] += _transmutateValue;
            //_valueForShadowPlacing += _totalShadowbarValue * _curve.Evaluate(_transmutatedShadow[((int)DIR.RIGHT)]);
        }
        //_valueForShadowPlacing = 0;
        //for (int i=0;i<4;i++)
        //{
        //    _valueForShadowPlacing += _totalShadowbarValue * _curve.Evaluate(_transmutatedShadow[i]);
        //}
        _transmutateValue = 0;
    }
    IEnumerator RevertAllTransmutation2()
    {
        Vector2 newScale;
        Vector3 newPosition;
        float valueDiff = 0;
        bool breakLoop = false;
        bool[] wasCheckedButNotMoved = new bool[4] { false, false, false, false };
        float scaleBound;
        while (_valueForShadowPlacing> 0&&!breakLoop)
        {
            for (int i = 0; i < 4; i++)
            {
                newScale = _shadowMask.localScale;
                newPosition = _shadowMask.localPosition;
                _revertTransmutateValue = Time.deltaTime * 2f;
                //newTransmutatedShadow = _transmutatedShadow[i] - _revertTransmutateValue;
                switch ((DIR)i)
                {
                    case DIR.LEFT:
                        {
                            if (_spriteMask.bounds.min.x <= _originalMinBound.x)
                            {
                                wasCheckedButNotMoved[i] = true;
                                if (wasCheckedButNotMoved[i] && wasCheckedButNotMoved[((int)DIR.RIGHT)])
                                {
                                    breakLoop = true;
                                }
                                continue;
                            }
                            scaleBound = math.remap(_originalMinBound.x, _originalMaxBound.x, 0, 1, _spriteMask.bounds.min.x);
                            if(scaleBound- _revertTransmutateValue<0)
                            {
                                _revertTransmutateValue += scaleBound - _revertTransmutateValue;
                            }
                            valueDiff = _curve.Evaluate(scaleBound) * _totalShadowbarValue - _curve.Evaluate(scaleBound - _revertTransmutateValue+ _lastOverShootValue) * _totalShadowbarValue;
                            Logger.Log($"{valueDiff} {_valueForShadowPlacing}");
                            if (valueDiff > _valueForShadowPlacing)
                            {
                                Logger.Log("Toobig");
                                _lastOverShootValue = _revertTransmutateValue;
                            }
                            else Logger.Log("OK");
                            _valueForShadowPlacing -= valueDiff;
                            if(math.abs( _valueForShadowPlacing-0)<0.001) _valueForShadowPlacing = 0;
                            newScale.x += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.RIGHT:
                        {
                            if (_spriteMask.bounds.max.x >= _originalMaxBound.x)
                            {
                                wasCheckedButNotMoved[i] = true;
                                if (wasCheckedButNotMoved[i] && wasCheckedButNotMoved[((int)DIR.LEFT)])
                                {
                                    breakLoop = true;
                                }
                                continue;
                            }
                            if (_spriteMask.bounds.max.x >= _originalMaxBound.x) continue;
                            Debug.Log("Right");
                            newScale.x += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.x += _revertTransmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.UP:
                        {
                            if (_spriteMask.bounds.max.y >= _originalMaxBound.y) continue;
                            newScale.y += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.y += _revertTransmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.DOWN:
                        {
                            if (_spriteMask.bounds.min.y <= _originalMinBound.y) continue;
                            newScale.y += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.y -= _transmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }

                }
                _transmutatedShadow[i] -= _revertTransmutateValue;
                yield return null;
            }
        }

        if (breakLoop)
        {
            _valueForShadowPlacing = 0;
        }
        if (_valueForShadowPlacing < 0) _valueForShadowPlacing = 0;
        for(int i=0;i<4;i++)
        {
            if (_transmutatedShadow[i] < 0) _transmutatedShadow[i] = 0;
        }
        Logger.Log("ENDED");
    }
    IEnumerator RevertAllTransmutationFromPlacedShadow2(float value)
    {
        Vector2 newScale;
        Vector3 newPosition;
        float valueDiff = 0;

        bool breakLoop = false;
        float scaleBound;
        bool[] wasCheckedButNotMoved=new bool[4]{ false,false,false,false};
        while (value > 0 && !breakLoop)
        {
            for (int i = 0; i < 4; i++)
            {
                newScale = _shadowMask.localScale;
                newPosition = _shadowMask.localPosition;
                _revertTransmutateValue = Time.deltaTime * 2f;
                switch ((DIR)i)
                {
                    case DIR.LEFT:
                        {
                            if (_spriteMask.bounds.min.x <= _originalMinBound.x)
                            {
                                wasCheckedButNotMoved[i] = true;
                                if (wasCheckedButNotMoved[i] && wasCheckedButNotMoved[((int)DIR.RIGHT)])
                                {
                                    breakLoop=true;
                                }
                                continue;
                            }
                            scaleBound = math.remap(_originalMinBound.x, _originalMaxBound.x, 0, 1, _spriteMask.bounds.min.x);
                            if (scaleBound - _revertTransmutateValue < 0)
                            {
                                _revertTransmutateValue += scaleBound - _revertTransmutateValue;
                            }
                            valueDiff = _curve.Evaluate(scaleBound) * _totalShadowbarValue - _curve.Evaluate(scaleBound - _revertTransmutateValue) * _totalShadowbarValue;
                            value -= valueDiff;
                            if (math.abs(value - 0) < 0.001) value = 0;
                            newScale.x += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.RIGHT:
                        {
                            if (_spriteMask.bounds.max.x >= _originalMaxBound.x)
                            {
                                wasCheckedButNotMoved[i] = true;
                                if (wasCheckedButNotMoved[i] && wasCheckedButNotMoved[((int)DIR.LEFT)])
                                {
                                    breakLoop = true;
                                }
                                continue;
                            }
                            Debug.Log("Right");
                            newScale.x += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.x += _revertTransmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.UP:
                        {
                            if (_spriteMask.bounds.max.y >= _originalMaxBound.y) continue;
                            newScale.y += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.y += _revertTransmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }
                    case DIR.DOWN:
                        {
                            if (_spriteMask.bounds.min.y <= _originalMinBound.y) continue;
                            newScale.y += _revertTransmutateValue;
                            _shadowMask.localScale = newScale;
                            newPosition.y -= _transmutateValue / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            break;
                        }

                }
                _transmutatedShadow[i] -= _revertTransmutateValue;
                yield return null;
            }
        }

        Logger.Log(value);
        Logger.Log("ENDED");
    }
    IEnumerator RevertAllTransmutation()
    {
        Vector2 newScale;
        Vector3 newPosition;
        bool _allAllClear = false;
        while(!_allAllClear)
        {
            _allAllClear = true;
            for (int i = 0; i < 4; i++)
            {
                if (_transmutatedShadow[i] > 0)
                {
                    newScale = _shadowMask.localScale;
                    newPosition = _shadowMask.localPosition;
                    _revertTransmutateValue = Time.deltaTime * 2f;
                    _transmutatedShadow[i] -= _revertTransmutateValue;
                    if (_transmutatedShadow[i] < 0)
                    {
                        _revertTransmutateValue += _transmutatedShadow[i];
                        _transmutatedShadow[i] = 0;
                    }
                    else if (_transmutatedShadow[i] == 0) continue;
                    _allAllClear = false;
                    switch ((DIR)i)
                    {
                        case DIR.LEFT:
                            {
                                newScale.x += _revertTransmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                _valueForShadowPlacing = _totalShadowbarValue  * _curve.Evaluate(_transmutatedShadow[i]);
                                break;
                            }
                        case DIR.RIGHT:
                            {
                                newScale.x += _transmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.x += _transmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }
                        case DIR.UP:
                            {
                                newScale.y += _transmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.y += _transmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }
                        case DIR.DOWN:
                            {
                                newScale.y += _transmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.y -= _transmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }
                            
                    }
                    
                }
                yield return null;
            }
        }
        //while (_transmutatedShadow[((int)DIR.LEFT)]>0)
        //{
        //    Vector2 newScale = _shadowMask.localScale;
        //    Vector3 newPosition = _shadowMask.localPosition;
        //    _transmutateValue = Time.deltaTime * 2f;
        //    _transmutatedShadow[((int)DIR.LEFT)] -= _transmutateValue;
        //    if(_transmutatedShadow[((int)DIR.LEFT)]<0)
        //    {
        //        _transmutateValue -= _transmutatedShadow[((int)DIR.LEFT)];
        //        _transmutatedShadow[((int)DIR.LEFT)] = 0;
        //    }
        //    newScale.x += _transmutateValue;
        //    _shadowMask.localScale = newScale;
        //    newPosition.x -= _transmutateValue / scaleToPoSrate;
        //    _shadowMask.localPosition = newPosition;
        //    yield return null;
        //}
    }
    IEnumerator RevertAllTransmutationFromPlacedShadow(float value)
    {
        Vector2 newScale;
        Vector3 newPosition;
        float diff = 0;
        Logger.Log(value);
        while(value>0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (_transmutatedShadow[i] > 0)
                {
                    newScale = _shadowMask.localScale;
                    newPosition = _shadowMask.localPosition;
                    _revertTransmutateValue = Time.deltaTime * 2f;
                    diff = _curve.Evaluate(_transmutatedShadow[i]) * _totalShadowbarValue - _curve.Evaluate(_transmutatedShadow[i] - _revertTransmutateValue) * _totalShadowbarValue;
                    Logger.Log($"diff:  {diff}");
                    if (diff > value* _revertTransmutateValue)
                    {
                        value-= diff;
                        Debug.Log(value);
                        restoredBarVal += diff;
                    }
                    else
                    {
                        continue;
                    }
                    _transmutatedShadow[i] -= _revertTransmutateValue;
                    if (_transmutatedShadow[i] < 0)
                    {
                        _revertTransmutateValue -= _transmutatedShadow[i];
                        _transmutatedShadow[i] = 0;
                        continue;
                    }
                    switch ((DIR)i)
                    {
                        case DIR.LEFT:
                            {
                                newScale.x += _revertTransmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }
                        case DIR.RIGHT:
                            {
                                newScale.x += _revertTransmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.x += _revertTransmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }
                        case DIR.UP:
                            {
                                newScale.y += _revertTransmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.y += _revertTransmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }
                        case DIR.DOWN:
                            {
                                newScale.y += _revertTransmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.y -= _transmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
                                break;
                            }

                    }

                }
                yield return null;
            }
        }
    }

}
