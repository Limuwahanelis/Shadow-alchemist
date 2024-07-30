using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShadowMissilesBoss : MonoBehaviour
{
    [SerializeField] List<Transform> _firePositions;
    [SerializeField] GameObject _missilePrefab;
    [SerializeField] Vector2 _missileDireciton;
    List<Missile> _availableMissiles = new List<Missile>();
    private void Update()
    {
    }
    public void FireMissiles()
    {
        for (int i = 0; i < _firePositions.Count; i++)
        {
            if (_availableMissiles.Count == 0)
            {
                Missile missile = Instantiate(_missilePrefab, _firePositions[i].position, _missilePrefab.transform.rotation, null).GetComponent<Missile>();
                missile.SetUp(_missileDireciton);
                missile.OnMissileCrushed.AddListener(AddMissileToList);
            }
            else
            {
                Missile missile = _availableMissiles[0];
                _availableMissiles.RemoveAt(0);
                missile.transform.position = _firePositions[i].position;
                missile.SetUp(_missileDireciton);
                missile.OnMissileCrushed.AddListener(AddMissileToList);
            }
        }
    }

    private void AddMissileToList(Missile missile)
    {
        missile.gameObject.SetActive(false);
        _availableMissiles.Add(missile);
        missile.OnMissileCrushed.RemoveListener(AddMissileToList);
    }
}
