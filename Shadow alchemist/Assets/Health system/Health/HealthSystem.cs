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
        ENEMY = 1,
        MISSILE = 2,
        TRAPS = 4,
        SHADOW_SPIKE=8,
        PLAYER=16,
        BOSS=32,
        ALL = ~0,
    }
    public Action<DamageInfo> OnHitEvent;
    public Action OnDeathEvent;
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;


    [SerializeField] protected bool isInvincible;
    [SerializeField] protected HealthBar hpBar;
    [SerializeField] protected int maxHP;
    [SerializeField] protected int currentHP;
    protected bool _isAlive=true;

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
        if (!_isAlive) return;
        if (currentHP <= 0) Kill();
    }
    public void TakeDamageSilent(DamageInfo info)
    {
        currentHP -= info.dmg;
        hpBar.SetHealth(currentHP);
        if (!_isAlive) return;
        if (currentHP <= 0) Kill();
    }
    public virtual void Kill()
    {
        _isAlive = false;
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
