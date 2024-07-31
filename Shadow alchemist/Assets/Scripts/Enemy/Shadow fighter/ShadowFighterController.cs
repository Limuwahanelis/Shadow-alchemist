using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowFighterController : EnemyController
{
    [Header("Shadow Fighter"), SerializeField] ControllableShadowIwthEnemy _originShadow;
    [SerializeField] ShadowFighterMovement _shadowFighterMovement;
    [SerializeField] PlayerDetection _frontDetection;
    [SerializeField] ShadowFighterCombat _shadowFighterCombat;
    [SerializeField] GameObject _sweatDropSprite;
    [SerializeField] EnemyChecks _enemyChecks;
    [SerializeField] float _minPlayerRange;
    [SerializeField] float _maxPlayerRange;
    [SerializeField] float _distanceFromShadowBounds;
    [SerializeField] float _maxPlayerEngageDistance;


    [Header("Patrol"),SerializeField] List<Transform> _patrolPoints=new List<Transform>();
    protected ShadowFighterContext _context;

    void Start()
    {
        if (_playerTransform == null) _playerTransform= ((PlayerController)FindFirstObjectByType(typeof(PlayerController))).transform;
        List<Type> states = AppDomain.CurrentDomain.GetAssemblies().SelectMany(domainAssembly => domainAssembly.GetTypes())
    .Where(type => typeof(EnemyState).IsAssignableFrom(type) && !type.IsAbstract).ToArray().ToList();
        _healthSystem.OnDeathEvent += ShadowFighterDeath;
        _healthSystem.OnWeakendStateReached += EnterWeakendState;
        _healthSystem.OnWeakendStateEnded += LeaveWeakendState;
        _healthSystem.OnHitEvent += TryStun;
        _healthSystem.OnHitEvent += Hit;
        _context = new ShadowFighterContext
        {
            enemyTransform = transform,
            ChangeEnemyState = ChangeState,
            animMan = _enemyAnimationManager,
            playerTransform = _playerTransform,
            coroutineHolder = this,
            originShadow = _originShadow,
            engageLevel = _enemyEngageLevel,
            frontPlayerDetection = _frontDetection,
            patrolPoints = _patrolPoints,
            movement = _shadowFighterMovement,
            combat = _shadowFighterCombat,
            patrolPointIndex = 0,
            distanceFromShadowBounds = _distanceFromShadowBounds,
            weakendStatus = _enemyWeakendStatus,
            enemyChecks = _enemyChecks,
            minPlayerRange = _minPlayerRange,
            maxPlayerRange = _maxPlayerRange,
            maxPlayerEngageDistance=_maxPlayerEngageDistance,
            WaitFrameAndPerformFunction=WaitFrameAndExecuteFunction,
            DestroyItself=DestroyItself,
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
    private void Hit(DamageInfo info)
    {
        _currentEnemyState.Hit(info);
    }
    private void TryStun(DamageInfo info)
    {
        if(info.damageType==HealthSystem.DamageType.SHADOW_SPIKE)
        {
            if(_enemyWeakendStatus.Status==EnemyWeakendStatus.WeakenStatus.WEAKEN)
            {
                EnemyState newState = GetState(typeof(ShadowFighterStateStunned));
                newState.SetUpState(_context);
                ChangeState(newState);
                _currentEnemyState = newState;
            }
        }
    }
    private void EnterWeakendState(DamageInfo info)
    {
        if (_enemyWeakendStatus.Status != EnemyWeakendStatus.WeakenStatus.NONE) return;
        _enemyWeakendStatus.Status = EnemyWeakendStatus.WeakenStatus.WEAKEN;
        if(info.damageType!=HealthSystem.DamageType.ENEMY && _healthSystem.CurrentHP>0)
        {
            if (info.damageType == HealthSystem.DamageType.SHADOW_SPIKE) // if weakend state is achieved by spike damage - stun enemy immediately
            {
                EnemyState newState = GetState(typeof(ShadowFighterStateStunned));
                newState.SetUpState(_context);
                ChangeState(newState);
                _currentEnemyState = newState;
            }
            else
            {
                EnemyState newState = GetState(typeof(ShadowFighterStatePushed));
                ChangeState(newState);
                ((ShadowFighterStatePushed)newState).SetUpState(_context, info);
            }
        }
        _sweatDropSprite.SetActive(true);

    }
    private void LeaveWeakendState()
    {
        if (_enemyWeakendStatus.Status!=EnemyWeakendStatus.WeakenStatus.WEAKEN) return;
        _enemyWeakendStatus.Status = EnemyWeakendStatus.WeakenStatus.NONE;
        _sweatDropSprite.SetActive(false);
    }
    public override void KillByLeavingShadow()
    {
        _healthSystem.OnDeathEvent -= ShadowFighterDeath;
        _enemyAnimationManager.SetAnimator(true);
        WaitFrameAndExecuteFunction(() =>
        {
            _sweatDropSprite.SetActive(false);
            EnemyState newState = GetState(typeof(ShadowFighterStateDead));
            ChangeState(newState);
            newState.SetUpState(_context);
        });

    }
    private void ShadowFighterDeath()
    {
        _healthSystem.OnDeathEvent -= ShadowFighterDeath;
        _originShadow.RemoveEnemyFromShadow(this);
        _enemyWeakendStatus.Status = EnemyWeakendStatus.WeakenStatus.NONE;
        _sweatDropSprite.SetActive(false);
        EnemyState newState = GetState(typeof(ShadowFighterStateDead));
        ChangeState(newState);
        newState.SetUpState(_context);
        _currentEnemyState = newState;
    }
    private void DestroyItself()
    {
        Destroy(gameObject);
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
    private void OnDestroy()
    {
        _healthSystem.OnHitEvent -= Hit;
        _healthSystem.OnHitEvent -= TryStun;
        _healthSystem.OnDeathEvent -= ShadowFighterDeath;
        _healthSystem.OnWeakendStateReached -= EnterWeakendState;
        _healthSystem.OnWeakendStateEnded -= LeaveWeakendState;
    }
}
