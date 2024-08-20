using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HealthSystem;

public class PlayerHealthSystem : HealthSystem,IPushable
{
    public Action<PushInfo> OnPushed;
    [SerializeField] float _invincibilityAfterHitDuration;
    [SerializeField] Collider2D[] _playerCols;
    private DamageType _invincibiltyType;
    private DamageType _pushInvincibiltyType;
    public Ringhandle pushHandle;
    public float pushForce=2f;
    private new void Start()
    {
        if (hpBar == null) return;
        hpBar.SetMaxHealth(maxHP);
        hpBar.SetHealth(maxHP);
    }
    public void SetInvincibility(DamageType invincibiltyType)
    {
        _invincibiltyType = invincibiltyType;
    }
    public void SetPushInvincibility(DamageType invincibiltyType)
    {
        _pushInvincibiltyType = invincibiltyType;
    }
    public override void TakeDamage(DamageInfo info)
    {
        if (currentHP>0)
        {
            if ((_invincibiltyType & info.damageType)== info.damageType) return;
            currentHP -= info.dmg;
            if(hpBar!=null) hpBar.SetHealth(currentHP);

            if (currentHP < 0) Kill();
            else OnHitEvent?.Invoke(info);
            //player.currentState.OnHit();
            StartCoroutine(InvincibilityCor());
        }

    }

    public override void Kill()
    {
        if(IsDeathEventSubscribedTo()) InvokeOnDeathEvent();
        else Destroy(gameObject);
    }
    IEnumerator PushCor(Collider2D[] colls)
    {
        yield return new WaitForSeconds(_invincibilityAfterHitDuration);
        RestoreCollisions(colls);

    }
    IEnumerator InvincibilityCor()
    {
        _invincibiltyType=DamageType.ALL;
        _pushInvincibiltyType = DamageType.ALL;
        yield return new WaitForSeconds(_invincibilityAfterHitDuration);
        _invincibiltyType = DamageType.NONE;
        _pushInvincibiltyType = DamageType.NONE;
    }
    public void Push(PushInfo pushInfo)
    {
        if ((_pushInvincibiltyType & pushInfo.pushType) == pushInfo.pushType) return;
        OnPushed?.Invoke(pushInfo);
        if(pushInfo.involvedColliders!=null) StartCoroutine(PushCor(pushInfo.involvedColliders));
    }
    public void IncreaseHealthBarMaxValue()
    {
        hpBar.SetMaxHealth(maxHP);
        hpBar.SetHealth(currentHP);
    }


}
