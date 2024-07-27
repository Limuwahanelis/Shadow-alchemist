using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [Header("Debug"), SerializeField] bool _printState;
    public GameObject MainBody=>_mainBody;
    [Header("Enemy common"),SerializeField] protected AnimationManager _enemyAnimationManager;
    [SerializeField] protected EnemyHealthSystem2 _healthSystem;
    [SerializeField] protected Transform _playerTransform;
    [SerializeField] protected GameObject _mainBody;
    [SerializeField] protected EnemyEngageLevel _enemyEngageLevel;
    [SerializeField] protected EnemyWeakendStatus _enemyWeakendStatus;
    protected Dictionary<Type, EnemyState> _enemyStates = new Dictionary<Type, EnemyState>();
    protected EnemyState _currentEnemyState;

    public EnemyState GetState(Type state)
    {
        return _enemyStates[state];
    }
    public void ChangeState(EnemyState newState)
    {
        if (_printState) Logger.Log(newState.GetType());
        _currentEnemyState.InterruptState();
        _currentEnemyState = newState;
    }
    public Coroutine WaitFrameAndExecuteFunction(Action function)
    {
        return StartCoroutine(WaitFrame(function));
    }
    public IEnumerator WaitFrame(Action function)
    {
        yield return new WaitForNextFrameUnit();
        function();
    }
    public virtual void KillByLeavingShadow()
    {

    }
}
