using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableShadowIwthEnemy : ControllableShadow
{
    
    [SerializeField] List<EnemyController> _enemies=new List<EnemyController>();
    [SerializeField] int dmgOutsideShadow;
    [SerializeField] float _timeToDealDmgOutside;
    [SerializeField] float _enemtInterferenceRange;
    private List<EnemyController> _enemiesOutside= new List<EnemyController>();
    private float _time=0;
    private DamageInfo _damageInfo;
    private EnemyController _enemyAtLeftBound;
    private EnemyController _enemyAtRightBound;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (_enemies.Count < 1) return;
        _enemyAtLeftBound = _enemies[0];
        _enemyAtRightBound = _enemies[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemiesOutside.Count == 0) return;
        _time += Time.deltaTime;
        if (_time >= _timeToDealDmgOutside)
        {
            for (int i = 0; i < _enemiesOutside.Count; i++)
            {
                _damageInfo = new DamageInfo(dmgOutsideShadow, HealthSystem.DamageType.ENEMY, transform.position);
                _enemiesOutside[i].GetComponent<IDamagable>().TakeDamage(_damageInfo);
            }
            _time = 0;
        }
    }
    public override void Transmutate(Vector2 directionTotakeFrom)
    {
        if(directionTotakeFrom==Vector2.zero) return;
        if (_enemies.Count >0)
        {
            if (_enemyAtRightBound == null) _enemyAtRightBound = _enemies[0];
            if (_enemyAtLeftBound == null) _enemyAtLeftBound = _enemies[0];
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].transform.position.x < _enemyAtLeftBound.transform.position.x) _enemyAtLeftBound = _enemies[i];
                if (_enemies[i].transform.position.x > _enemyAtRightBound.transform.position.x) _enemyAtRightBound = _enemies[i];
            }
            if (directionTotakeFrom == Vector2.right)
            {
                if (_enemyAtRightBound.GetComponent<EnemyWeakendStatus>().Status != EnemyWeakendStatus.WeakenStatus.STUNNED)
                {
                    if (ShadowBounds.max.x < _enemyAtRightBound.transform.position.x + _enemtInterferenceRange) return;
                }
            }
            else if (directionTotakeFrom == Vector2.left)
            {
                if (_enemyAtLeftBound.GetComponent<EnemyWeakendStatus>().Status != EnemyWeakendStatus.WeakenStatus.STUNNED)
                {
                    if (ShadowBounds.min.x > _enemyAtLeftBound.transform.position.x - _enemtInterferenceRange) return;
                }
            }
        }
        base.Transmutate(directionTotakeFrom);
    }
    public void RemoveEnemyFromShadow(EnemyController enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
        if(_enemiesOutside.Contains(enemy)) 
        {
            _enemiesOutside.Remove(enemy);
        }
        if(enemy==_enemyAtLeftBound) _enemyAtLeftBound = null;
        if(enemy==_enemyAtRightBound) _enemyAtRightBound = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb)
        {
            EnemyController enemy = rb.GetComponent<EnemyController>();
            if (enemy)
            {
                if (!_enemies.Contains(enemy))
                {
                    if (_enemiesOutside.Contains(enemy))
                    {
                        _enemiesOutside.Remove(enemy);
                        _enemies.Add(enemy);
                    }
                    else Logger.Error("Enemy with origin to another shadow enterd not his shadow");
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb)
        {
            EnemyController enemy = rb.GetComponent<EnemyController>();
            if(enemy)
            {
                if(_enemies.Contains(enemy))
                {
                    _enemies.Remove(enemy);
                    //enemy.KillByLeavingShadow();
                    if (enemy.GetComponent<EnemyWeakendStatus>().Status==EnemyWeakendStatus.WeakenStatus.STUNNED) enemy.KillByLeavingShadow();
                    else _enemiesOutside.Add(enemy);
                    if (enemy == _enemyAtLeftBound) _enemyAtLeftBound = null;
                    if (enemy == _enemyAtRightBound) _enemyAtRightBound = null;
                }
            }
        }
    }
}
