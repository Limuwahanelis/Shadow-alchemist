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

    protected override void Start()
    {
        base.Start();
        _points = ((PolygonCollider2D)_shadowCollider).points;
        _originalPositions = ((PolygonCollider2D)_shadowCollider).points;
        _points[0].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[0].y = transform.InverseTransformPoint(_spriteMask.bounds.max).y;
        _points[1].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[1].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        _points[2].x = transform.InverseTransformPoint(_spriteMask.bounds.min).x;
        _points[2].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        _points[3].x = transform.InverseTransformPoint(_spriteMask.bounds.max).x;
        _points[3].y = transform.InverseTransformPoint(_spriteMask.bounds.min).y;
        tang = math.abs(_points[0].y - _points[3].y) / math.abs(_points[1].x - _points[3].x);
        Logger.Log(tang);
        ((PolygonCollider2D)_shadowCollider).points = _points;
    }
    #region ShadowMove
    public override void MoveShadow(float moveSpeed, Vector2 direction)
    {
        base.MoveShadow(moveSpeed, direction);
        
        for (int i = 0; i < _points.Length; i++)
        {           
            _points[i] += _shadowShift-_lastFrameShadowShift;
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
        base.Transmutate(directionTotakeFrom);
        AdjustCollider();
    }
    protected override bool RevertNonSegmentStep(DIR revertDirection)
    {
        bool value = base.RevertNonSegmentStep(revertDirection);
        AdjustCollider();
        return value;
    }
    protected override bool RevertSegmentStep(DIR revertDirection)
    {
        bool value = base.RevertSegmentStep(revertDirection);
        AdjustCollider();
        return value;
    }
    #endregion
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
}
