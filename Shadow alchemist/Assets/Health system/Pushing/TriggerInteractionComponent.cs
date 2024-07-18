using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteractionComponent : MonoBehaviour,IDamager
{
    
    [SerializeField] protected HealthSystem.DamageType _pushType;
    [SerializeField] protected HealthSystem.DamageType _damageType;
    [SerializeField] protected bool _pushCollidingObject;
    [SerializeField] protected bool _damageCollidingObject;
    [SerializeField] protected int damage;

    public Vector3 Position { get => transform.position; }

    private void Start()
    {

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (_pushCollidingObject)
        {
            IPushable toPush = collision.transform.GetComponentInParent<IPushable>();
            if (toPush != null) toPush.Push(_pushType);
        }
        if (_damageCollidingObject)
        {
            IDamagable toDamage = collision.transform.GetComponentInParent<IDamagable>();
            if (toDamage != null) toDamage.TakeDamage(new DamageInfo(damage, _damageType, transform.position,this));
        }
    }
    public void ResumeCollisons(Collider2D[] playerCols)
    {
        foreach (Collider2D col in playerCols)
        {
            Physics2D.IgnoreCollision(col, GetComponent<Collider2D>(), false);
        }

    }

    public void PreventCollisions(Collider2D[] playerCols)
    {
        foreach (Collider2D col in playerCols)
        {
            Physics2D.IgnoreCollision(col, GetComponent<Collider2D>(), true);
        }
    }
}
