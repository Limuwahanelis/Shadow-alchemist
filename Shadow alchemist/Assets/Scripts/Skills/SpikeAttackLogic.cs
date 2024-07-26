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
    private Coroutine _attackCor;
    public void StartAttackCor()
    {
        _attackCor = StartCoroutine(AttackCor());
    }
    public void StopAttckCor()
    {
        StopCoroutine(_attackCor);
    }
    public IEnumerator AttackCor()
    {
        List<Collider2D> hitEnemies = Physics2D.OverlapBoxAll(_spikeAttackPos.position, _spikeAttackSize, 0, _enemyLayer).ToList();
        int index = 0;
        for (; index < hitEnemies.Count; index++)
        {
            IDamagable tmp = hitEnemies[index].GetComponentInParent<IDamagable>();
            if (tmp != null) tmp.TakeDamage(new DamageInfo(_damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));
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
                    if (tmp != null) tmp.TakeDamage(new DamageInfo(_damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));
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
