using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmutableShadow : MonoBehaviour
{
    [SerializeField] float scaleToPoSrate=2f;
    [SerializeField] float _shadowBarValue;
    [SerializeField] Transform _shadowTransform;
    [SerializeField] Transform _lefBorder;
    [SerializeField] Transform _rightBorder;

    private void Update()
    {
        //Transmutate(Vector2.left);
    }
    public void Transmutate()
    {
        //throw new System.NotImplementedException();

    }

    public void Transmutate(Vector2 directionTotakeFrom)
    {
        if (directionTotakeFrom == Vector2.left)
        {

            if (_shadowTransform.localScale.x < 0) return;
            Vector2 newScale = transform.localScale;
            Vector3 newPosition = transform.localPosition;
            newScale.x -= Time.deltaTime * 2f;
            _shadowTransform.localScale = newScale;
            newPosition.x += (Time.deltaTime * 2f) / scaleToPoSrate;
            _shadowTransform.position = newPosition;
        }
    }
}
