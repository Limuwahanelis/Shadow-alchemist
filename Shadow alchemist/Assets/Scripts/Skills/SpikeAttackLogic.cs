using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class SpikeAttackLogic : MonoBehaviour
{
    [SerializeField] bool _debug;
    [SerializeField] Transform _spikeAttackPos;
    [SerializeField] Vector2 _spikeAttackSize;
    [SerializeField] int _damage;
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] AnimationManager _animationManager;
    [SerializeField] float _durationOnHit;
    private Coroutine _attackCor;
    private float _timeToDespawnSpike;
    private float _time;
    private bool _isDespawning = false;
    private bool _isFirstHit = true;
    private Coroutine _despawnCor;
    private ControllableShadow _originShadow;
    private bool _isInFullShadow;
    IDamagable _awaitedDamageable;
    private void Update()
    {
        if (_isDespawning) return;
        if (_isInFullShadow)
        {
        }
        else
        {
            if (!_originShadow.ShadowBounds.Contains(transform.position))
            {
                StopAttckCor();
                _animationManager.PlayAnimation("Reverse spike");
                _isDespawning = true;
                StartCoroutine(DespawnCor());
                return;
            }
        }
        _time += Time.deltaTime;
        if(_time > _timeToDespawnSpike)
        {
            StopAttckCor();
            _animationManager.PlayAnimation("Reverse spike");
            _isDespawning = true;
            StartCoroutine(DespawnCor());
        }
    }
    public void SetUp(float time, bool isInFullShadow)
    {
        _isInFullShadow = isInFullShadow;
        _timeToDespawnSpike = time;
        StartAttackCor();
    }
    public void SetUp(float time,ControllableShadow shadow)
    {
        _originShadow = shadow;
        _timeToDespawnSpike = time;
        StartAttackCor();
    }
    public void StartAttackCor()
    {
        _attackCor = StartCoroutine(AttackCor());
    }
    public void StopAttckCor()
    {
        StopCoroutine(_attackCor);
    }
    private void StartDespawnCor(IDamagable damagable)
    {
        Logger.Log("despawn");
        
        StartCoroutine(DespawnCor());
    }
    public IEnumerator DespawnCor()
    {
        yield return new WaitForSeconds(_animationManager.GetAnimationLength("Spike"));
        if(_awaitedDamageable!=null) _awaitedDamageable.OnDeath -= StartDespawnCor;
        Destroy(gameObject);
    }
    public IEnumerator AttackCor()
    {
        List<Collider2D> hitEnemies = Physics2D.OverlapBoxAll(_spikeAttackPos.position, _spikeAttackSize, 0, _enemyLayer).ToList();
        int index = 0;
        for (; index < hitEnemies.Count; index++)
        {
            IDamagable tmp = hitEnemies[index].GetComponentInParent<IDamagable>();
            if (tmp != null)
            {
                if (_isFirstHit)
                {

                    if (hitEnemies[index].GetComponentInParent<EnemyWeakendStatus>().Status == EnemyWeakendStatus.WeakenStatus.WEAKEN)
                    {
                        _isFirstHit = false;
                        _timeToDespawnSpike += _durationOnHit;
                        tmp.OnDeath += StartDespawnCor;
                        _awaitedDamageable = tmp;
                        Logger.Log("HIt");
                    }
                }
                tmp.TakeDamage(new DamageInfo(_damage, PlayerHealthSystem.DamageType.SHADOW_SPIKE, transform.position));

            }
        }
        yield return null;
        while (true)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(_spikeAttackPos.position, _spikeAttackSize, 0, _enemyLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!hitEnemies.Contains(colliders[i]))
                {
                    hitEnemies.Add(colliders[i]);
                    IDamagable tmp = colliders[i].GetComponentInParent<IDamagable>();
                    if (tmp != null)
                    {
                        if (_isFirstHit)
                        {

                            if (hitEnemies[index].GetComponentInParent<EnemyWeakendStatus>().Status == EnemyWeakendStatus.WeakenStatus.WEAKEN)
                            {
                                _isFirstHit = false;
                                _timeToDespawnSpike += _durationOnHit;
                                tmp.OnDeath += StartDespawnCor;
                                _awaitedDamageable = tmp;
                                Logger.Log("HIt");
                            }
                        }
                        tmp.TakeDamage(new DamageInfo(_damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));


                    }
                }
            }
            yield return null;
        }
    }
    private void OnDestroy()
    {
       // damagable.OnDeath -= StartDespawnCor;
    }
    private void OnDrawGizmosSelected()
    {
        if (_debug)
        {
            if (_spikeAttackPos != null) Gizmos.DrawWireCube(_spikeAttackPos.position, _spikeAttackSize);
        }
    }
}
