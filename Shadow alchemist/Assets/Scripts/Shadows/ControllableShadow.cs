using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

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
    private List<PlacableShadow> _placedShadows= new List<PlacableShadow>();
    private float[] _transmutatedShadow = new float[4]; // stores how much scale was reduced from orignal
    private float _transmutateValue = 0;
    private Vector2 _originalPosition;
    private Coroutine _revertMoveCor;
    public float _valueForShadowPlacing;
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
        Destroy(_placedShadows[_placedShadows.Count - 1].gameObject);
        _placedShadows.RemoveAt(_placedShadows.Count - 1);
        
    }
    #endregion
    public void RevertTransmutationFromPlacedShadow(float _shadowBarCost)
    {

    }
    public void RevertTransmutation()
    {
        StartCoroutine(RevertAllTransmutation());
    }

    public void Transmutate()
    {
        
    }

    public void Transmutate(Vector2 directionTotakeFrom)
    {
        if (directionTotakeFrom == Vector2.left)
        {
            if (_shadowMask.localScale.x < 0) return;
            Vector2 newScale = _shadowMask.localScale;
            Vector3 newPosition = _shadowMask.localPosition;
            _transmutateValue = Time.deltaTime * 2f;
            newScale.x -= _transmutateValue;
            _shadowMask.localScale = newScale;
            newPosition.x += _transmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            _transmutatedShadow[((int)DIR.LEFT)] += _transmutateValue;
        }
        if (directionTotakeFrom == Vector2.right)
        {
            if (_shadowMask.localScale.x < 0) return;
            Vector2 newScale = _shadowMask.localScale;
            Vector3 newPosition = _shadowMask.localPosition;
            _transmutateValue = Time.deltaTime * 2f;
            newScale.x -= _transmutateValue;
            _shadowMask.localScale = newScale;
            newPosition.x -= _transmutateValue / scaleToPoSrate;
            _shadowMask.localPosition = newPosition;
            _transmutatedShadow[((int)DIR.RIGHT)] += _transmutateValue;
        }
        _valueForShadowPlacing = 0;
        for (int i=0;i<4;i++)
        {
            _valueForShadowPlacing += _totalShadowbarValue * _curve.Evaluate(_transmutatedShadow[i]);
        }
        
        _transmutateValue = 0;
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
                    _transmutateValue = Time.deltaTime * 2f;
                    _transmutatedShadow[i] -= _transmutateValue;
                    if (_transmutatedShadow[i] < 0)
                    {
                        _transmutateValue -= _transmutatedShadow[i];
                        _transmutatedShadow[i] = 0;
                        continue;
                    }
                    _allAllClear = false;
                    switch ((DIR)i)
                    {
                        case DIR.LEFT:
                            {
                                newScale.x += _transmutateValue;
                                _shadowMask.localScale = newScale;
                                newPosition.x -= _transmutateValue / scaleToPoSrate;
                                _shadowMask.localPosition = newPosition;
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


}
