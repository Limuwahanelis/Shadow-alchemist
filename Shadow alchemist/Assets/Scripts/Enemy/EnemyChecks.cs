using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecks : MonoBehaviour
{
    public bool IsTouchingGround => _isTouchingGround;
    [SerializeField] Transform _groundCheckPos;
    [SerializeField] float _groundCheckLength;
    [SerializeField] LayerMask _groundMask;
    private bool _isTouchingGround;
    RaycastHit2D _hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _hit=Physics2D.Raycast(_groundCheckPos.position, -transform.up , _groundCheckLength, _groundMask);
        if(_hit)
        {
            _isTouchingGround=true;
        }
        else _isTouchingGround = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheckPos) Gizmos.DrawRay(_groundCheckPos.position, -_groundCheckPos.up * _groundCheckLength);
    }
}
