using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterContext : EnemyContext
{
    public ControllableShadowIwthEnemy originShadow;
    public PlayerDetection frontPlayerDetection;
    public PlayerDetection backPlayerDetection;
    public List<Transform> patrolPoints;
    public ShadowFighterMovement movement;
    public bool isEngagingWithPlayer;
    public int patrolPointIndex;
    public float minPlayerRange;
    public float maxPlayerRange;
    public float maxPlayerEngageDistance;
    public float distanceFromShadowBounds;
}
