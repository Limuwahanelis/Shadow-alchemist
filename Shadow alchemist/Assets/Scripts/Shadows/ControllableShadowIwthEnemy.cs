using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableShadowIwthEnemy : ControllableShadow
{
    
    [SerializeField] List<EnemyController> _enemies=new List<EnemyController>();
    [SerializeField] int dmgOutsideShadow;
    [SerializeField] float _timeToDealDmgOutside;
    private List<EnemyController> _enemiesOutside= new List<EnemyController>();
    private float _time=0;
    private DamageInfo _damageInfo;
    // Start is called before the first frame update
    void Start()
    {
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
    public void RemoveEnemyFromShadow(EnemyController enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
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
                }
            }
        }
    }
}
