using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState 
{
    public delegate EnemyState GetState(Type state);
    protected EnemyContext _enemyContext;
    protected GetState _getType;

    public EnemyState(GetState function)
    {
        _getType = function;
    }
    public virtual void SetUpState(EnemyContext context)
    {
        _enemyContext = context;
    }

    public abstract void Update();
    public virtual void FixedUpdate() { }
    public virtual void Move(Vector2 direction) { }
    public virtual void Attack(PlayerCombat.AttackModifiers modifier = PlayerCombat.AttackModifiers.NONE) { }

    public virtual void Hit(DamageInfo damageInfo) { ChangeState(_enemyContext.enemyHitState.GetType()); }

    public abstract void InterruptState();
    public void ChangeState(Type newStateType)
    {
        EnemyState state = _getType(newStateType);
        _enemyContext.ChangeEnemyState(state);
        state.SetUpState(_enemyContext);
    }
}
