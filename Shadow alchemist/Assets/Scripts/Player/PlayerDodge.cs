using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] Collider2D _enemyCol;
    public void SetEnemyCollider(bool isActive)
    {
        _enemyCol.enabled = isActive;
    }
}
