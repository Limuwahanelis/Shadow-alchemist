using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShadowsInteractions : MonoBehaviour
{
    public ControllableShadow Shadow => _shadow;
    private ControllableShadow _shadow;
    public ITransmutableSadow TransmutableShadow => _transmutablkeShadow;
    public PlacableShadow ShadowToPlace => _currentlyPlacingShadow;
    private PlacableShadow _currentlyPlacingShadow;
    private ITransmutableSadow _transmutablkeShadow;
    private float _shadowBarValue;
    [SerializeField] GameObject _shadowPrefab;
    public void SpawnShadow()
    {
        _currentlyPlacingShadow = Instantiate(_shadowPrefab,transform.position+transform.right,_shadowPrefab.transform.rotation,null).GetComponent<PlacableShadow>();

        Logger.Log($"{ _currentlyPlacingShadow}  {ShadowToPlace}");
    }
    public void DespawnShadow()
    {
        if(_currentlyPlacingShadow == null) return;
        Destroy(_currentlyPlacingShadow.gameObject);
        _currentlyPlacingShadow = null;
    }
    public void PlaceShadow()
    {
        _currentlyPlacingShadow = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ControllableShadow shadow = collision.attachedRigidbody.GetComponent<ControllableShadow>();
        if (shadow != null) _shadow = shadow; Logger.Log($"{_shadow}  {Shadow}");
        ITransmutableSadow shadow2 = collision.attachedRigidbody.GetComponent<ITransmutableSadow>();
        if(shadow2!=null)_transmutablkeShadow = shadow2;
    }
}
