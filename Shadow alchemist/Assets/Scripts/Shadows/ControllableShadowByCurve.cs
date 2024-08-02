using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class ControllableShadowByCurve : ControllableShadow
{
    private AnimationCurve _curve;
    private Vector2[] _points;
    private Vector2[] _originalPositions;
    private Vector2 _lastFrameShadowShift;
    protected override void Start()
    {
        _curve = new AnimationCurve();
        _points = ((PolygonCollider2D)_shadowCollider).points;
        _originalPositions = ((PolygonCollider2D)_shadowCollider).points;
#if UNITY_EDITOR
        Vector2[] cutPoints = ((PolygonCollider2D)_shadowCollider).points[1..(_points.Length-1)];
        Keyframe[] keyframes = new Keyframe[cutPoints.Length];

        _curve.ClearKeys();
        
        for (int i=0;i<cutPoints.Length;i++)
        {
            keyframes[i].time = cutPoints[i].x;
            keyframes[i].value = cutPoints[i].y;
        }
        _curve.keys = keyframes;
        for (int i=0;i<cutPoints.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(_curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(_curve, i, AnimationUtility.TangentMode.Linear);
        }
#endif
        base.Start();

    }

    public override void MoveShadow(float moveSpeed, Vector2 direction)
    {
        base.MoveShadow(moveSpeed, direction);

        for (int i = 0; i < _points.Length; i++)
        {
            _points[i] += _shadowShift - _lastFrameShadowShift;
            _originalPositions[i] += _shadowShift - _lastFrameShadowShift;
        }
        ((PolygonCollider2D)_shadowCollider).points = _points;
        _lastFrameShadowShift = _shadowShift;
    }
    public override void Transmutate(Vector2 directionTotakeFrom)
    {
        if (_isReverting) return;
        if (directionTotakeFrom == Vector2.zero) return;
        base.Transmutate(directionTotakeFrom);
        AdjustCollider();
    }
    protected override void RevertShadowMoveStep()
    {
        base.RevertShadowMoveStep();
        for (int i = 0; i < _points.Length; i++)
        {
            _points[i] += _shadowShift - _lastFrameShadowShift;
            _originalPositions[i] += _shadowShift - _lastFrameShadowShift;
        }
             ((PolygonCollider2D)_shadowCollider).points = _points;
        _lastFrameShadowShift = _shadowShift;
    }
    protected override IEnumerator RevertNonBarTransmutation()
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        _revertTransmutateValue = Time.deltaTime * _transmutationSpeed;
        _isReverting = true;
        bool isAllClear = false;
        while (!isAllClear)
        {
            newScale = _shadowMask.localScale;
            newPosition = _shadowMask.localPosition;
            switch (_lastTransmutationDirection)
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
                        isAllClear = RevertHorizontalTransmutation(DIR.LEFT,false);
                        break;

                    }
                case DIR.RIGHT:
                    {
                        if (!_isHorizontal) break;
                        isAllClear = RevertHorizontalTransmutation(DIR.RIGHT, false);
                        break;
                    }
                case DIR.NONE: isAllClear = true;break;

            }
            AdjustColliderForRevert();
            SetValueForShadowBar();
            yield return null;
        }
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;
        _lastTransmutationDirection = DIR.NONE;
        _isReverting = false;
    }
    protected override IEnumerator RevertLastBarTransmutation()
    {
        Vector2 newScale = _shadowMask.localScale;
        Vector2 newPosition = _shadowMask.localPosition;
        _revertTransmutateValue = Time.deltaTime * _transmutationSpeed;
        bool isClear = false;
        DIR tmp = _shadowSegmentsList[_shadowSegmentsList.Count - 1];
        _shadowSegmentsList.RemoveAt(_shadowSegmentsList.Count - 1);
        _isReverting = true;
        while (!isClear)
        {
            switch (tmp)
            {
                case DIR.LEFT:
                    {
                        if (!_isHorizontal) break;
                        isClear = RevertHorizontalTransmutation(DIR.LEFT, true);
                        if(isClear)
                        {
                               _segmentsTakenPerSide[((int)DIR.LEFT)]--;
                                _segmentsTaken--;
                        }
                        break;
                    }
                case DIR.RIGHT:
                    {
                        if (!_isHorizontal) break;
                        isClear = RevertHorizontalTransmutation(DIR.RIGHT,true);
                        if(isClear)
                        {
                                _segmentsTakenPerSide[((int)DIR.RIGHT)]--;
                                _segmentsTaken--;
                        }
                        break;
                    }
                case DIR.UP:
                    {
                        if (_isHorizontal) break;
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
            AdjustColliderForRevert();
            SetValueForShadowBar();
            yield return null;
        }
        _isReverting = false;
        _valueForShadowPlacing = _segmentsTaken - _placedShadows.Count;

    }

    /// <summary>
    /// Returns true if shadow mask max bound or min bound reached last segment 
    /// </summary>
    /// <param name="revertDirection"></param>
    /// <returns></returns>
    private void AdjustColliderForRevert()
    {
        _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;

        for (int j = 2; j < _points.Length/2; j++)
            {
                if (_originalPositions[j].x > transform.InverseTransformPoint(_spriteMask.bounds.min).x && _originalPositions[j].x < transform.InverseTransformPoint(_spriteMask.bounds.max).x)
                {
                    _points[j] = _originalPositions[j];
                    Logger.Log(j);
                    continue;
                }
                if (_points[j - 1].x < _points[j].x) _points[j].x = _points[j - 1].x;
                _points[j].y = _curve.Evaluate(_points[j].x - _shadowShift.x);
            }
        for(int j= _points.Length / 2;j< _points.Length-2;j++)
        {
            if (_originalPositions[j].x > transform.InverseTransformPoint(_spriteMask.bounds.min).x && _originalPositions[j].x < transform.InverseTransformPoint(_spriteMask.bounds.max).x)
            {
                _points[j] = _originalPositions[j];
                Logger.Log(j);
                continue;
            }
            if (_points[j +1].x > _points[j].x) _points[j].x = _points[j + 1].x;
            _points[j].y = _curve.Evaluate(_points[j].x - _shadowShift.x);
        }
        ((PolygonCollider2D)_shadowCollider).points = _points;
    }
    private void AdjustCollider()
    {

        _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[_points.Length - 1].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[_points.Length - 1].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        _points[_points.Length - 2].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;

        for (int i = 2; i < _points.Length - 2; i++)
        {
            if (_points[i - 1].x > _points[i].x) _points[i].x = _points[i - 1].x;
            _points[i].y = _curve.Evaluate(_points[i].x - _shadowShift.x);
        }

        for (int i = _points.Length - 3; i >= 2; i--)
        {
            if (_points[i + 1].x < _points[i].x) _points[i].x = _points[i + 1].x;
            _points[i].y = _curve.Evaluate(_points[i].x - _shadowShift.x);
        }
             ((PolygonCollider2D)_shadowCollider).points = _points;


    }
}
