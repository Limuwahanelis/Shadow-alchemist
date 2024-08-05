using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static ControllableShadow;

public class ControllableShadowCustomShape : ControllableShadow
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] bool _useLerp;
    private Vector2[] _points;
    private Vector2[] _originalPositions;
    private Vector2 _lastFrameShadowShift;
    private int _mostLeftPointIndex;
    private int _mostRightPointIndex;
    protected override void Start()
    {
        _curve = new AnimationCurve();
        _points = ((PolygonCollider2D)_shadowCollider).points;
        _originalPositions = ((PolygonCollider2D)_shadowCollider).points;
#if UNITY_EDITOR
        Vector2[] cutPoints = ((PolygonCollider2D)_shadowCollider).points[1..(_points.Length - 1)];
        Keyframe[] keyframes = new Keyframe[cutPoints.Length];

        _curve.ClearKeys();

        for (int i = 0; i < cutPoints.Length; i++)
        {
            keyframes[i].time = cutPoints[i].x;
            keyframes[i].value = cutPoints[i].y;
        }
        _curve.keys = keyframes;
        for (int i = 0; i < cutPoints.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(_curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(_curve, i, AnimationUtility.TangentMode.Linear);
        }
#endif
        FindMostOutsidePoints();
        base.Start();

    }
    private void FindMostOutsidePoints()
    {
        _mostLeftPointIndex = 0;
        _mostRightPointIndex = 0;
        for(int i=0;i<_points.Length;i++) 
        {
            if (_points[i].x < _points[ _mostLeftPointIndex].x) _mostLeftPointIndex = i;
            if (_points[i].x > _points[_mostRightPointIndex].x) _mostRightPointIndex = i;
        }
    }
    #region ShadowMove
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
    #endregion
    #region ShadowTransmutation
    public override void Transmutate(Vector2 directionTotakeFrom)
    {
        if (_isReverting) return;
        if (directionTotakeFrom == Vector2.zero) return;
        base.Transmutate(directionTotakeFrom);
        AdjustCollider();
    }

    protected override bool RevertNonSegmentStep(DIR revertDirection)
    {
        bool value = base.RevertNonSegmentStep(revertDirection);
        AdjustColliderForRevert();
        return value;
    }
    protected override bool RevertSegmentStep(DIR revertDirection)
    {
        bool value = base.RevertSegmentStep(revertDirection);
        AdjustColliderForRevert();
        return value;
    }
    #endregion
    private void AdjustColliderForRevert()
    {
        _points[_mostLeftPointIndex].x = _shadowCollider.transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[_mostRightPointIndex].x = _shadowCollider.transform.InverseTransformPoint(_spriteMask.bounds.max).x;

        for (int i = 0; i < _points.Length / 2; i++)
        {
            if (i == _mostLeftPointIndex) continue;
            if (_originalPositions[i].x > transform.InverseTransformPoint(_spriteMask.bounds.min).x && _originalPositions[i].x < transform.InverseTransformPoint(_spriteMask.bounds.max).x)
            {
                _points[i] = _originalPositions[i];
                continue;
            }
            if (_points[_mostLeftPointIndex].x > _originalPositions[i].x) _points[i].x = _points[_mostLeftPointIndex].x;
            if (_originalPositions[i + 1].x > _originalPositions[i].x) _points[i].y = Vector2.Lerp(_originalPositions[i], _originalPositions[i + 1], 1 - (math.abs(_points[i].x - _originalPositions[i + 1].x) / math.distance(_originalPositions[i].x, _originalPositions[i + 1].x))).y;
            //else if (_originalPositions[i - 1].x > _originalPositions[i].x) _points[i].y = Vector2.Lerp(_originalPositions[i], _originalPositions[i + 1], 1 - (math.abs(_points[i].x - _originalPositions[i + 1].x) / math.distance(_originalPositions[i].x, _originalPositions[i + 1].x))).y; 
        }

        for (int i = _points.Length / 2; i < _points.Length; i++)
        {
            if (_originalPositions[i].x >= transform.InverseTransformPoint(_spriteMask.bounds.min).x && _originalPositions[i].x <= transform.InverseTransformPoint(_spriteMask.bounds.max).x)
            {
                _points[i] = _originalPositions[i];
                continue;
            }
            if (_points[_mostRightPointIndex].x < _originalPositions[i].x) _points[i].x = _points[_mostRightPointIndex].x;
            if (_originalPositions[i - 1].x < _originalPositions[i].x) _points[i].y = Vector2.Lerp(_originalPositions[i], _originalPositions[i - 1], 1 - (math.abs(_points[i].x - _originalPositions[i - 1].x) / math.distance(_originalPositions[i].x, _originalPositions[i - 1].x))).y;
        }
        ((PolygonCollider2D)_shadowCollider).points = _points;
    }
    private void AdjustCollider()
    {

        _points[_mostLeftPointIndex].x = _shadowCollider.transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[_mostRightPointIndex].x = _shadowCollider.transform.InverseTransformPoint(_spriteMask.bounds.max).x;

        for (int i = 0; i < _points.Length - 2; i++)
        {
            if (_points[_mostLeftPointIndex].x > _points[i].x) _points[i].x = _points[_mostLeftPointIndex].x;
            if (_originalPositions[i + 1].x > _originalPositions[i].x) _points[i].y = Vector2.Lerp(_originalPositions[i], _originalPositions[i + 1],1- (math.abs(_points[i].x- _originalPositions[i + 1].x)/math.distance(_originalPositions[i].x, _originalPositions[i + 1].x))).y;
        }

        for (int i = _points.Length - 1; i >= 1; i--)
        {
            if (_points[_mostRightPointIndex].x < _points[i].x) _points[i].x = _points[_mostRightPointIndex].x;
            if (_originalPositions[i - 1].x < _originalPositions[i].x) _points[i].y = Vector2.Lerp(_originalPositions[i], _originalPositions[i - 1], 1 - (math.abs(_points[i].x - _originalPositions[i - 1].x) / math.distance(_originalPositions[i].x, _originalPositions[i - 1].x))).y;
        }
             ((PolygonCollider2D)_shadowCollider).points = _points;


    }
}
