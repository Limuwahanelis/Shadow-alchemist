using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerDetectionConstant : PlayerDetection
{
    public Action<Vector3> OnPlayerStay;
    private void OnTriggerStay2D(Collider2D collision)
    {
        playerPos = collision.transform.position;
        OnPlayerStay?.Invoke(playerPos);
    }
}
