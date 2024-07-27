using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    int dmg = 16;
    private void OnCollisionStay2D(Collision2D collision)
    {
        IDamagable player = collision.transform.GetComponent<PlayerHealthSystem>();
        IPushable toPush = collision.transform.GetComponent<PlayerHealthSystem>();
        toPush.Push(PlayerHealthSystem.DamageType.TRAPS);
        player.TakeDamage(new DamageInfo(dmg,HealthSystem.DamageType.TRAPS,transform.position));
    }
}
