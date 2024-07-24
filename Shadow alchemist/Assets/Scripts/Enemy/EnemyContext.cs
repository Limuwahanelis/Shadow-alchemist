using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyContext
{
    public Action<EnemyState> ChangeEnemyState;
    public AnimationManager animMan;
    public EnemyState enemyHitState;
    public Transform playerTransform;
    public Transform enemyTransform;
    public Rigidbody2D rb;
    public MonoBehaviour coroutineHolder;
}
