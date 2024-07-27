using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem2 : HealthSystem
{
    public Action<DamageInfo> OnWeakendStateReached;
    public Action OnWeakendStateEnded;
    [Header("Weakend State"), SerializeField] int _damageRequiredToReachWeakendState;
    [SerializeField] float _weakendStateDuration;
    private int _wekaendStateNum = 1;
    private Coroutine _weakendCoroutine=null;
    public override void TakeDamage(DamageInfo info)
    {
        currentHP -= info.dmg;
        hpBar.SetHealth(currentHP);
        if(currentHP<maxHP-( _wekaendStateNum*_damageRequiredToReachWeakendState))
        {
            OnWeakendStateReached?.Invoke(info);
            _wekaendStateNum++;
            if (_weakendCoroutine == null)
            {
                _weakendCoroutine = StartCoroutine(WeakenedStateCor());
            }
            else
            {
                StopCoroutine(_weakendCoroutine);
                _weakendCoroutine = StartCoroutine(WeakenedStateCor());
            }
        }
        OnHitEvent?.Invoke(info);
        if (!_isAlive) return;
        if (currentHP <= 0) Kill();
    }
    IEnumerator WeakenedStateCor()
    {
        yield return new WaitForSeconds(_weakendStateDuration);
        OnWeakendStateEnded?.Invoke();
        _weakendCoroutine = null;
    }
}
