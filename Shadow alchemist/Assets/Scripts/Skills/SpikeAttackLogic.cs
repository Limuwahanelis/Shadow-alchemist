using System.Collections;
using System.Collections.Generic;
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
    private void Update()
    {
        if (_isDespawning) return;
        _time += Time.deltaTime;
        if(_time > _timeToDespawnSpike)
        {
            _animationManager.PlayAnimation("Reverse spike");
            _isDespawning = true;
            StartCoroutine(DespawnCor());
        }
    }
    public void SetUp(float time)
    {
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
    public IEnumerator DespawnCor()
    {
        yield return new WaitForSeconds(_animationManager.GetAnimationLength("Spike"));
        StopAttckCor();
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
                tmp.TakeDamage(new DamageInfo(_damage, PlayerHealthSystem.DamageType.SHADOW_SPIKE, transform.position));
                if (_isFirstHit)
                {
                    _isFirstHit = false;
                    _timeToDespawnSpike += _durationOnHit;
                }
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
                        tmp.TakeDamage(new DamageInfo(_damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));
                        if (_isFirstHit)
                        {
                            _isFirstHit = false;
                            _timeToDespawnSpike += _durationOnHit;
                        }

                    }
                }
            }
            yield return null;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (_debug)
        {
            if (_spikeAttackPos != null) Gizmos.DrawWireCube(_spikeAttackPos.position, _spikeAttackSize);
        }
    }
}
