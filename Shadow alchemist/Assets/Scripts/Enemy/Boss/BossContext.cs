using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossContext : EnemyContext
{
    public Transform[] teleportPoints; // 0 -left side, 1 - up, 2 - right side, 3 - player left, 4 - player right
    public int indexOfTeleportPos;
    public Action OnFightStarted;
    public SpawnShadowMissilesBoss topMissilesSpawner;
    public SpawnShadowMissilesBoss leftMissilesSpawner;
    public SpawnShadowMissilesBoss rightMissilesSpawner;
    public BossController.BossPhase currentPhase;
    public BossCombat combat;
    public BossMovement movement;
    public BossChargeInfo chargeInfo;
    public Transform leftChargeStop;
    public Transform rightChargeStop;
    public Collider2D lanceCollider;
    public Collider2D bossCollider;
    public GameObject sweatDrop;
    public HealthSystem healthSystem;
}
