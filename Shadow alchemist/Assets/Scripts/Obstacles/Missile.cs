using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Missile : MonoBehaviour
{
    public UnityEvent<Missile> OnMissileCrushed;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] float _speed;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] AnimationManager _animManager;
    [SerializeField] Collider2D _col;
    [SerializeField] int _damage;
    private Vector2 _direction;
    private bool _crushed = false;
    private DamageInfo _damageInfo;
    private PushInfo _pushInfo;

    public void SetUp(Vector2 direction)
    {
        gameObject.SetActive(true);
        _direction = direction;
        _animManager.PlayAnimation("Start");
        _crushed = false;
        _col.enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        _damageInfo = new DamageInfo(_damage,HealthSystem.DamageType.MISSILE,transform.position);
        _pushInfo = new PushInfo(HealthSystem.DamageType.MISSILE,transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_crushed) return;
        _rb.MovePosition(_rb.position+_direction*Time.deltaTime*_speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_playerLayer == (_playerLayer | (1 << collision.collider.gameObject.layer)))
        {
            _pushInfo.pushPosition = transform.position;
            _damageInfo.dmgPosition = transform.position;
            collision.collider.GetComponentInParent<PlayerHealthSystem>().Push(_pushInfo);
            collision.collider.GetComponentInParent<PlayerHealthSystem>().TakeDamage(_damageInfo);
            return;
        }
        if (_crushed) return;
        _crushed = true;
        _animManager.PlayAnimation("Crush");
        _col.enabled = false;
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
}
