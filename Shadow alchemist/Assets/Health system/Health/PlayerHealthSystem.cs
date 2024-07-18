using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : HealthSystem,IPushable
{

    public Action<Vector3, IDamager> OnPushed;
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
        hpBar.SetHealth(currentHP);
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
            if (_invincibiltyType==info.damageType || _invincibiltyType==DamageType.ALL) return;
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
        if (OnDeathEvent == null) Destroy(gameObject);
        else OnDeathEvent.Invoke();
    }
    IEnumerator PushCor(IDamager pusher)
    {
        yield return new WaitForSeconds(_invincibilityAfterHitDuration);
        if (pusher != null)
        {
            pusher.ResumeCollisons(_playerCols);
        }
    }
    IEnumerator InvincibilityCor()
    {
        _invincibiltyType=DamageType.ALL;
        _pushInvincibiltyType = DamageType.ALL;
        yield return new WaitForSeconds(_invincibilityAfterHitDuration);
        _invincibiltyType = DamageType.NONE;
        _pushInvincibiltyType = DamageType.NONE;
    }

    public void Push(PlayerHealthSystem.DamageType damageType, IDamager pusher = null)
    {
        if (currentHP > 0)
        {
            if (_pushInvincibiltyType == damageType || _pushInvincibiltyType == DamageType.ALL) return;
            //_playerMovement.PushPlayer(pushHandle.GetVector() * pushForce,pusher);
            OnPushed?.Invoke(Vector3.zero, pusher);
            if (pusher != null) pusher.PreventCollisions(_playerCols);
            StartCoroutine(PushCor(pusher));
        }
    }
    public void Push(Vector3 pushDirection, DamageType damageType)
    {
        if (currentHP > 0)
        {
            if (_pushInvincibiltyType == damageType || _pushInvincibiltyType == DamageType.ALL) return;
            //_playerMovement.PushPlayer(direction, pushHandle.GetVector() * pushForce, null);
            OnPushed?.Invoke(pushDirection, null);
            StartCoroutine(PushCor(null));
        }
    }
    public void Push(Vector3 pushDirection, DamageType damageType, IDamager pusher)
    {
        if (currentHP > 0)
        {
            if (_pushInvincibiltyType == damageType || _pushInvincibiltyType == DamageType.ALL) return;
            //_playerMovement.PushPlayer(direction, pushHandle.GetVector() * pushForce, pusher);
            OnPushed?.Invoke(pushDirection, pusher);
            StartCoroutine(PushCor(pusher));
        }
    }
    public void IncreaseHealthBarMaxValue()
    {
        hpBar.SetMaxHealth(maxHP);
        hpBar.SetHealth(currentHP);
    }
}
