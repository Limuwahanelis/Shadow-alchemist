using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowFighterController : EnemyController
{
    [Header("Shadow Fighter"), SerializeField] ControllableShadowIwthEnemy _originShadow;
    [SerializeField] ShadowFighterMovement _shadowFighterMovement;
    [SerializeField] PlayerDetection _frontDetection;
    [SerializeField] ShadowFighterCombat _shadowFighterCombat;
    [SerializeField] float _minPlayerRange;
    [SerializeField] float _maxPlayerRange;
    [SerializeField] float _distanceFromShadowBounds;
    [SerializeField] float _maxPlayerEngageDistance;
    [Header("Patrol"),SerializeField] List<Transform> _patrolPoints=new List<Transform>();
    protected ShadowFighterContext _context;

    void Start()
    {
        List<Type> states = AppDomain.CurrentDomain.GetAssemblies().SelectMany(domainAssembly => domainAssembly.GetTypes())
    .Where(type => typeof(EnemyState).IsAssignableFrom(type) && !type.IsAbstract).ToArray().ToList();

        _context = new ShadowFighterContext
        {
            enemyTransform = transform,
            ChangeEnemyState = ChangeState,
            animMan = _enemyAnimationManager,
            playerTransform = _playerTransform,
            coroutineHolder = this,
            originShadow = _originShadow,
            frontPlayerDetection = _frontDetection,
            patrolPoints = _patrolPoints,
            movement = _shadowFighterMovement,
            combat = _shadowFighterCombat,
            patrolPointIndex = 0,
            distanceFromShadowBounds=_distanceFromShadowBounds,
            minPlayerRange = _minPlayerRange,
            maxPlayerRange = _maxPlayerRange,
            maxPlayerEngageDistance=_maxPlayerEngageDistance,
        };
        EnemyState.GetState getState = GetState;
        foreach (Type state in states)
        {
            _enemyStates.Add(state, (EnemyState)Activator.CreateInstance(state, getState));
        }
        EnemyState newState = GetState(typeof(ShadowFighterStateIdle));
        newState.SetUpState(_context);
        _currentEnemyState = newState;
    }

    void Update()
    {
        _currentEnemyState?.Update();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxPlayerEngageDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(MainBody.transform.position + Vector3.right * MainBody.transform.localScale.x * _maxPlayerRange, transform.position + Vector3.right * MainBody.transform.localScale.x * _maxPlayerRange + Vector3.down * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(MainBody.transform.position + Vector3.right * MainBody.transform.localScale.x * _minPlayerRange, transform.position + Vector3.right * MainBody.transform.localScale.x * _minPlayerRange + Vector3.down * 2);
    }
}
