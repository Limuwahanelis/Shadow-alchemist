using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerHealthSystem;

public class HealthSystem : MonoBehaviour,IDamagable
{
    [Flags]
    public enum DamageType
    {
        NONE = 0,
        ENEMY = 2,
        MISSILE = 4,
        TRAPS = 8,
        ALL = 16,
    }
    public Action<DamageInfo> OnHitEvent;
    public Action OnDeathEvent;

    [SerializeField] protected bool isInvincible;
    [SerializeField] protected HealthBar hpBar;
    [SerializeField] protected int maxHP;
    [SerializeField] protected int currentHP;


    // Start is called before the first frame update
    protected void Start()
    {
        hpBar.SetMaxHealth(maxHP);
        currentHP = maxHP;
        hpBar.SetHealth(currentHP);
    }
    public virtual void TakeDamage(DamageInfo info)
    {
        currentHP -= info.dmg;
        hpBar.SetHealth(currentHP);
        OnHitEvent?.Invoke(info);
        if (currentHP <= 0) Kill();
    }

    public virtual void Kill()
    {
        if (OnDeathEvent == null)
        {
            Destroy(gameObject);
            Destroy(hpBar.gameObject);
        }
        else OnDeathEvent.Invoke();
    }

    public void MissileLeftCollider(Collider2D missile)
    {
        
    }
}
