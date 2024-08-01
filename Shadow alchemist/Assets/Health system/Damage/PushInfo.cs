using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushInfo
{
    public HealthSystem.DamageType pushType;
    public Vector3 pushPosition;
    public Collider2D[] involvedColliders;

    public PushInfo(HealthSystem.DamageType pushType,Vector3 pushpos, Collider2D[] involvedColliders=null) 
    {
        this.pushType = pushType;
        this.pushPosition = pushpos;
        this.involvedColliders = involvedColliders;
    }
}
