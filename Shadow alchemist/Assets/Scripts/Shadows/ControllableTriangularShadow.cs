using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class ControllableTriangularShadow : ControllableShadow
{
    private Vector2[] _points;
    private Vector2[] _originalPositions;
    private float tang;
    private Vector2 _lastFrameShadowShift;

    private void Start()
    {
        base.Start();
        _points = ((PolygonCollider2D)_shadowCollider).points;
        _originalPositions = ((PolygonCollider2D)_shadowCollider).points;
        _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[1].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;//_originalPositions[1].y + math.abs(_originalPositions[1].x - _points[1].x) * 0.5543f;//tan 29 degree;
        _points[2].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[2].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        _points[3].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[3].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        tang = math.abs(_points[0].y - _points[3].y) / math.abs(_points[1].x - _points[3].x);
        Logger.Log(tang);
        ((PolygonCollider2D)_shadowCollider).points = _points;
    }
    private void AdjustCollider()
    {
        _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[0].y = _originalPositions[3].y + math.abs(_originalPositions[1].x - _points[3].x) * tang;
        _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[1].y = _originalPositions[1].y + math.abs(_originalPositions[1].x - _points[1].x) * tang;
        _points[2].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[2].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        _points[3].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[3].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        ((PolygonCollider2D)_shadowCollider).points = _points;
    }
   public override void MoveShadow(float moveSpeed, Vector2 direction)
    {
        if (_revertMoveCor != null)
        {
            StopCoroutine(_revertMoveCor);
            _revertMoveCor = null;
        }


        if (_isHorizontal)
        {
            
            if (direction == Vector2.right)
            {
                if (_shadow.transform.position.x< _rightBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.x += direction.x * moveSpeed * Time.deltaTime;
                }

            }
            if (direction == Vector2.left)
            {
                if (_shadow.transform.position.x> _lefBorder.position.x)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.x += direction.x * moveSpeed * Time.deltaTime;
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
                    _shadowShift.y += direction.y * moveSpeed * Time.deltaTime;
                }
            }
            if (direction == Vector2.down)
            {
                if (_shadow.transform.position.y + _shadow.transform.localScale.y / 2 > _lowerBorder.position.y)
                {
                    _shadow.transform.Translate(direction * moveSpeed * Time.deltaTime);
                    _shadowShift.y += direction.y * moveSpeed * Time.deltaTime;
                }
            }
           
        }
        
        for (int i = 0; i < _points.Length; i++)
        {           
            _points[i] += _shadowShift-_lastFrameShadowShift;
            _originalPositions[i] += _shadowShift - _lastFrameShadowShift;
        }
        ((PolygonCollider2D)_shadowCollider).points = _points;
        _lastFrameShadowShift = _shadowShift;
    }

    public override void Transmutate(Vector2 directionTotakeFrom)
    {
        base.Transmutate(directionTotakeFrom);
        AdjustCollider();
    }
    protected override IEnumerator RevertLastBarTransmutation()
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        bool isClear = false;
        DIR tmp = _shadowSegmentsList[_shadowSegmentsList.Count - 1];
        _shadowSegmentsList.RemoveAt(_shadowSegmentsList.Count - 1);
        while (!isClear)
        {
            switch (tmp)
            {
                case DIR.LEFT:
                    {
                        if (!_isHorizontal) break;
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.x += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.x -= _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.min.x <= _segments[_segmentsTakenPerSide[((int)DIR.LEFT)] - 1].position.x)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.LEFT)] - 1].position).x - _shadow.InverseTransformPoint(_spriteMask.bounds.min).x;
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
                        if (!_isHorizontal) break;
                        _revertTransmutateValue = Time.deltaTime * 2f;
                        newScale.x += _revertTransmutateValue;
                        _shadowMask.localScale = newScale;
                        newPosition.x += _revertTransmutateValue / scaleToPoSrate;
                        _shadowMask.localPosition = newPosition;
                        if (_spriteMask.bounds.max.x >= _segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)] + 1].position.x)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_spriteMask.bounds.max).x - _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)] + 1].position).x;
                            newScale.x -= diff;
                            _shadowMask.localScale = newScale;
                            newPosition.x -= diff / scaleToPoSrate;
                            _shadowMask.localPosition = newPosition;
                            _segmentsTakenPerSide[((int)DIR.RIGHT)]--;
                            _segmentsTaken--;
                        }
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
                        if (_spriteMask.bounds.max.y >= _segments[_segmentsTakenPerSide[((int)DIR.UP)] - 1].position.y)
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
                        if (_spriteMask.bounds.min.y <= _segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.DOWN)] + 1].position.y)
                        {
                            isClear = true;

                            float diff = _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.DOWN)] + 1].position).y - _shadow.InverseTransformPoint(_spriteMask.bounds.min).y;
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
            AdjustCollider();
            SetValueForShadowBar();
            yield return null;
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;

    }
    protected override IEnumerator RevertNonBarTransmutation()
    {
        Vector2 newScale = _shadowMask.localScale;
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
                            float diff = _shadow.InverseTransformPoint(_segments[_segmentsTakenPerSide[((int)DIR.UP)]].position).y - _shadow.InverseTransformPoint(_spriteMask.bounds.max).y;
                            Logger.Log(diff);
                            if (math.abs(diff - 0) < 0.001f || diff < 0) continue;
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

                            break;

                        }
                    case DIR.RIGHT:
                        {
                            if (!_isHorizontal) break;
                            float diff = _shadow.InverseTransformPoint(_segments[_segments.Count - 1 - _segmentsTakenPerSide[((int)DIR.RIGHT)]].position).x - _shadow.InverseTransformPoint(_spriteMask.bounds.max).x;
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
                AdjustCollider();
                yield return null;
            }
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;
        _lastTransmutationDirection = DIR.NONE;
    }
}
