using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthSystem : HealthSystem
{
    private DamageType _invincibiltyType;
    private DamageType _pushInvincibiltyType;
    [SerializeField] float _invincibilityAfterHitDuration;
    [SerializeField] Collider2D[] _enemyCols;
    public override void TakeDamage(DamageInfo info)
    {
        if (currentHP > 0)
        {
            if (_invincibiltyType == info.damageType || _invincibiltyType == DamageType.ALL) return;
            currentHP -= info.dmg;
            if (hpBar != null) hpBar.SetHealth(currentHP);

            if (currentHP <= 0) Kill();
            else OnHitEvent?.Invoke(info);
            StartCoroutine(InvincibilityCor(info.damager));
        }

    }
    IEnumerator InvincibilityCor(IDamager damager)
    {
        damager.PreventCollisions(_enemyCols);
        //_invincibiltyType = DamageType.ALL;
        //_pushInvincibiltyType = DamageType.ALL;
        yield return new WaitForSeconds(_invincibilityAfterHitDuration);
        damager.ResumeCollisons(_enemyCols);
        //_invincibiltyType = DamageType.NONE;
        //_pushInvincibiltyType = DamageType.NONE;
    }

}
