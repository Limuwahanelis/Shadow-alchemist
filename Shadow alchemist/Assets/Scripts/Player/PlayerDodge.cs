using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{

    [Tooltip("Collider esponsible for collisions with enemies"), SerializeField] Collider2D _enemyCol;
    [SerializeField] PlayerHealthSystem _healthSystem;
    public void SetEnemyCollider(bool isActive)
    {
        _enemyCol.enabled = isActive;
    }

    public void SetInvincibility(HealthSystem.DamageType type)
    {
        _healthSystem.SetInvincibility(type);
    }
    public void SetPushInvincibility(HealthSystem.DamageType type)
    {
        _healthSystem.SetPushInvincibility(type);
    }
}
