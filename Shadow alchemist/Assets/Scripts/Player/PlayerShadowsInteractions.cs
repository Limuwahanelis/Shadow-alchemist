using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShadowsInteractions : MonoBehaviour
{
    public ControllableShadow Shadow => _shadow;
    private ControllableShadow _shadow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ControllableShadow shadow = collision.attachedRigidbody.GetComponent<ControllableShadow>();
        if (shadow != null) _shadow = shadow;
    }
}
