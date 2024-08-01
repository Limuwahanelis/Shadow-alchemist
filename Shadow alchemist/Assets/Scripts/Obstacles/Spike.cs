using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    int dmg = 16;
    private DamageInfo _damageInfo;
    private PushInfo _pushInfo;
    private void Start()
    {
        _damageInfo = new DamageInfo(dmg,HealthSystem.DamageType.TRAPS,transform.position);
        _pushInfo = new PushInfo(HealthSystem.DamageType.TRAPS, transform.position);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        IDamagable player = collision.transform.GetComponent<PlayerHealthSystem>();
        IPushable toPush = collision.transform.GetComponent<PlayerHealthSystem>();
        if(toPush!=null) toPush.Push(_pushInfo);
        if(player!=null)player.TakeDamage(_damageInfo);
    }
}
