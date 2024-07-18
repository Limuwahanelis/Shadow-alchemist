using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableShadow : MonoBehaviour
{
    [SerializeField] Transform _lefBorder;
    [SerializeField] Transform _rightBorder;
    [SerializeField] GameObject _shadowMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveShadow(1, Vector2.left);
    }

    public void MoveShadow(float moveSpeed,Vector2 direction)
    {
        if(direction==Vector2.right)
            if (_shadowMask.transform.position.x - _shadowMask.transform.localScale.x / 2 < _lefBorder.position.x) _shadowMask.transform.Translate(direction * moveSpeed * Time.deltaTime);
        if (direction == Vector2.left)
            if (_shadowMask.transform.position.x + _shadowMask.transform.localScale.x / 2 > _rightBorder.position.x) _shadowMask.transform.Translate(direction * moveSpeed * Time.deltaTime);

    }
}
