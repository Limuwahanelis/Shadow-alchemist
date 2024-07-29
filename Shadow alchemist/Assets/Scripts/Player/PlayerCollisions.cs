using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] Collider2D _floorCollider;
    [SerializeField] Collider2D _jumpEnemyCollider;
    public void SetPlayerFloorCollider(bool value)
    {
        _floorCollider.enabled = value;
    }
}
