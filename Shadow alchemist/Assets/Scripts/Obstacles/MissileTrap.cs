using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTrap : MonoBehaviour
{
    [SerializeField] GameObject _missilePrefab;
    [SerializeField] float _missileCooldown;
    [SerializeField] Vector2 _missileDireciton;
    [SerializeField] Transform _missileSpawnPoint;
    List<Missile> _availableMissiles= new List<Missile>();
    private float _time;
    private void Update()
    {
        _time += Time.deltaTime;
        if(_time>_missileCooldown)
        {
            FireMissile();
            _time = 0;
        }
    }
    public void FireMissile()
    {
        if(_availableMissiles.Count==0)
        {
            Missile missile = Instantiate(_missilePrefab, _missileSpawnPoint.position, _missilePrefab.transform.rotation, null).GetComponent<Missile>();
            missile.SetUp(_missileDireciton);
            missile.OnMissileCrushed.AddListener(AddMissileToList);
        }
        else
        {
            Missile missile = _availableMissiles[0];
            _availableMissiles.RemoveAt(0);
            missile.transform.position = _missileSpawnPoint.position;
            missile.SetUp(_missileDireciton);
            missile.OnMissileCrushed.AddListener(AddMissileToList);
        }
        
    }

    private void AddMissileToList(Missile missile)
    {
        missile.gameObject.SetActive(false);
        _availableMissiles.Add(missile);
        missile.OnMissileCrushed.RemoveListener(AddMissileToList);
    }
}
