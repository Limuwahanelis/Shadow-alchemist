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
       bool value= base.RevertNonSegmentStep(revertDirection);
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
