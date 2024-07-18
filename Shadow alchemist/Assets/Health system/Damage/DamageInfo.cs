using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public int dmg;
    public HealthSystem.DamageType damageType;
    public Vector3 dmgPosition;
    public IDamager damager;
    public DamageInfo(int dmg, HealthSystem.DamageType damageType,Vector3 dmgPosition,IDamager damager  = null) 
    {
        this.dmg = dmg;
        this.damageType = damageType;
        this.dmgPosition = dmgPosition;
        this.damager = damager;
    }
}
