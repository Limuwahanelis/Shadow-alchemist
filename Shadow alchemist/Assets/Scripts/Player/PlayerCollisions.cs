using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] Collider2D _floorCollider;
    [SerializeField] Collider2D _jumpEnemyCollider;

    [Header("Normal collliders"), SerializeField] Collider2D _groundCol;
    [SerializeField] Collider2D _enemyCol;
    [Header("Crouch colliders"), SerializeField] Collider2D _crouchGroundCol;
    [SerializeField] Collider2D _crouchEnemyCol;
    public void SetPlayerFloorCollider(bool value)
    {
        _floorCollider.enabled = value;
    }

    public void SetCrouchColliiers(bool value)
    {
        _crouchGroundCol.enabled = value;
        _crouchEnemyCol.enabled = value;
    }
    public void SetNormalColliders(bool value)
    {
        _groundCol.enabled = value;
        _enemyCol.enabled = value;
    }
}
