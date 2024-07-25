using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableShadowIwthEnemy : ControllableShadow
{
    [SerializeField] List<EnemyController> _enemies=new List<EnemyController>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    Destroy(enemy.gameObject);
                }
            }
        }
    }
}
