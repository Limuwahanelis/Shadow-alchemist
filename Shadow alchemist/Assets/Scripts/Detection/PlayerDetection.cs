using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerDetection : MonoBehaviour
{
    public UnityEvent OnPlayerDetectedUnity;
    public UnityEvent OnPlayerLeftdUnity;
    public Action OnPlayerDetected;
    public Action OnPlayerLeft;
    public Vector3 playerPos;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnPlayerDetected?.Invoke();
        OnPlayerDetectedUnity?.Invoke();
        playerPos = collision.transform.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerPos = Vector3.zero;
        OnPlayerLeftdUnity?.Invoke();
        OnPlayerLeft?.Invoke();
    }
}
