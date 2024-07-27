using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterContext : EnemyContext
{
    public Action DestroyItself;
    public ControllableShadowIwthEnemy originShadow;
    public PlayerDetection frontPlayerDetection;
    public PlayerDetection backPlayerDetection;
    public List<Transform> patrolPoints;
    public ShadowFighterMovement movement;
    public ShadowFighterCombat combat;
    public EnemyChecks enemyChecks;
    public bool isEngagingWithPlayer;
    public int patrolPointIndex;
    public float minPlayerRange;
    public float maxPlayerRange;
    public float maxPlayerEngageDistance;
    public float distanceFromShadowBounds;
}
