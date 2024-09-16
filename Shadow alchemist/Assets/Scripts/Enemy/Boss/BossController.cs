using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnemyWeakendStatus;

public class BossController : EnemyController
{
    public enum BossPhase
    {
        MISSILES_TOP,NORMAL_ATTACK,MISSLES_SIDES,CHARGE
    }
    [Header("Boss"),SerializeField] BossCombat _combat;
    [SerializeField] HealthSystem _healthSys;
    [SerializeField] BossMovement _movement;
    [SerializeField] GameObject _sweatDrop;
    [SerializeField] GameObject _gameWonPanel;
    [SerializeField] Collider2D _bossCollider;
    [Header("Charge"),SerializeField] BossChargeInfo _chargeInfo;
    [SerializeField] Transform _leftChargeStop;
    [SerializeField] Transform _rightChargeStop;
    [SerializeField] Collider2D _lanceCollider;
    [Header("Teleportation"),SerializeField] Transform[] _teleportationPoints;

    [Header("Missiles"), SerializeField] SpawnShadowMissilesBoss _leftMissileSpawner;
    [SerializeField] SpawnShadowMissilesBoss _rightMissileSpawner;
    [SerializeField] SpawnShadowMissilesBoss _upMissileSpawner;
    
    protected BossContext _context;

    void Start()
    {
        _healthSys.OnHitEvent += TryStun;
        _healthSys.OnDeath += BossDeath;
        List<Type> states = AppDomain.CurrentDomain.GetAssemblies().SelectMany(domainAssembly => domainAssembly.GetTypes())
    .Where(type => typeof(EnemyState).IsAssignableFrom(type) && !type.IsAbstract).ToArray().ToList();

        _context = new BossContext
        {
            ChangeEnemyState = ChangeState,
            animMan = _enemyAnimationManager,
            enemyTransform = transform,
            playerTransform = _playerTransform,
            engageLevel = _enemyEngageLevel,
            weakendStatus = _enemyWeakendStatus,
            currentPhase = BossPhase.MISSILES_TOP,
            teleportPoints = _teleportationPoints,
            topMissilesSpawner = _upMissileSpawner,
            leftMissilesSpawner = _leftMissileSpawner,
            rightMissilesSpawner = _rightMissileSpawner,
            combat=_combat,
            movement = _movement,
            coroutineHolder = this,
            chargeInfo = _chargeInfo,
            leftChargeStop = _leftChargeStop,
            rightChargeStop = _rightChargeStop,
            lanceCollider = _lanceCollider,
            bossCollider= _bossCollider,
            sweatDrop =_sweatDrop,
            healthSystem=_healthSys,
        };
        EnemyState.GetState getState = GetState;
        foreach (Type state in states)
        {
            _enemyStates.Add(state, (EnemyState)Activator.CreateInstance(state, getState));
        }
        EnemyState newState = GetState(BossStateInitialPhase.StateType);
        newState.SetUpState(_context);
        _currentEnemyState = newState;
    }

    void Update()
    {
        if (PauseSettings.IsGamePaused) return;
        _currentEnemyState?.Update();
    }
    private void FixedUpdate()
    {
        if (PauseSettings.IsGamePaused) return;
        _currentEnemyState?.FixedUpdate();
    }
    private void BossDeath(IDamagable damagable)
    {
        _gameWonPanel.SetActive(true);
        EnemyState newState = GetState(BossStateDead.StateType);
        newState.SetUpState(_context);
        ChangeState(newState);
    }
    private void TryStun(DamageInfo info)
    {
        if (info.damageType == HealthSystem.DamageType.SHADOW_SPIKE)
        {
            if (_enemyWeakendStatus.Status == EnemyWeakendStatus.WeakenStatus.WEAKEN)
            {
                _currentEnemyState.Hit(info);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Logger.Log(collision.rigidbody.gameObject);
        if(collision.rigidbody.GetComponent<PlacableShadow>())
        {
            collision.rigidbody.GetComponent<PlacableShadow>().ForceDestroy();
            _currentEnemyState.Hit(new DamageInfo());

        }
    }
    private void OnDestroy()
    {
        _healthSys.OnHitEvent -= TryStun;
    }
}
