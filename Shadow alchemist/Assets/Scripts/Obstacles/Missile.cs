using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Missile : MonoBehaviour,IDamager
{
    public UnityEvent<Missile> OnMissileCrushed;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _speed;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] AnimationManager _animManager;
    [SerializeField] Collider2D _col;
    private Vector2 _direction;
    private bool _crushed = false;

    public Vector3 Position => transform.position;

    public void SetUp(Vector2 direction)
    {
        gameObject.SetActive(true);
        _direction = direction;
        _animManager.PlayAnimation("Start");
        _crushed = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_crushed) return;
        _rb.MovePosition(_rb.position+_direction*Time.deltaTime*_speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_playerLayer == (_playerLayer | (1 << collision.collider.gameObject.layer)))
        {
            collision.collider.GetComponentInParent<PlayerHealthSystem>().Push(HealthSystem.DamageType.MISSILE, this);
            collision.collider.GetComponentInParent<PlayerHealthSystem>().TakeDamage(new DamageInfo(12, HealthSystem.DamageType.MISSILE, transform.position));
            return;
        }
        if (_crushed) return;
        _crushed = true;
        _animManager.PlayAnimation("Crush");
        StartCoroutine(DestroyAfterCrush(_animManager.GetAnimationLength("Crush")));
    }


    IEnumerator DestroyAfterCrush(float time)
    {
        yield return new WaitForSeconds(time);
        if(OnMissileCrushed==null) Destroy(gameObject);
        else OnMissileCrushed?.Invoke(this);
        
    }
    private void OnDestroy()
    {
        OnMissileCrushed.RemoveAllListeners();
    }

    public void ResumeCollisons(Collider2D[] playerCols)
    {
        foreach (Collider2D col in playerCols)
        {
            Physics2D.IgnoreCollision(col, _col, false);
        }
    }

    public void PreventCollisions(Collider2D[] playerCols)
    {
        foreach(Collider2D col in playerCols)
        {
            Physics2D.IgnoreCollision(col, _col,true);
        }
    }
}
