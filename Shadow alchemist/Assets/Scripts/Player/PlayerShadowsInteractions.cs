using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShadowsInteractions : MonoBehaviour
{
    public Action OnControllableShadowLeft;
    public Action PlacingShadowDespawned;
    public ControllableShadow Shadow => _shadow;
    private ControllableShadow _shadow;
    public ITransmutableSadow TransmutableShadow => _transmutablkeShadow;
    public PlacableShadow ShadowToPlace => _currentlyPlacingShadow;
    public float ShadowPlacingSpeed => _shadowPlacingSpeed;
    [SerializeField] float _shadowPlacingSpeed;
    [SerializeField] ShadowBar _shadowBar;
    private PlacableShadow _currentlyPlacingShadow;
    private ITransmutableSadow _transmutablkeShadow;
    private float _shadowBarValue;
    private void Update()
    {
        if(Shadow==null) return;
        _shadowBar.SetValue(Shadow.TakenShadowBarValueFromTransmutation);
    }
    public void SpawnShadow(GameObject _shadowPrefab)
    {
        _currentlyPlacingShadow = Instantiate(_shadowPrefab,transform.position+transform.right,_shadowPrefab.transform.rotation,null).GetComponent<PlacableShadow>();
        _currentlyPlacingShadow.SetParentShadow(_shadow, _shadow.ShadowCollider);
        _currentlyPlacingShadow.OnLeftParentShadow += ForceDespawn;
        Logger.Log($"{ _currentlyPlacingShadow}  {ShadowToPlace}");
    }
    public void ForceDespawn(PlacableShadow newShadow)
    {
        PlacingShadowDespawned?.Invoke();
        _currentlyPlacingShadow.OnLeftParentShadow -= ForceDespawn;
        Destroy(_currentlyPlacingShadow.gameObject);
        _currentlyPlacingShadow = null;
    }
    public void DespawnShadow()
    {
        if(_currentlyPlacingShadow == null) return;
        _currentlyPlacingShadow.OnLeftParentShadow -= ForceDespawn;
        Destroy(_currentlyPlacingShadow.gameObject);
        _currentlyPlacingShadow = null;
    }
    public bool PlaceShadow()
    {
        if (_currentlyPlacingShadow.CanBePlaced)
        {
            Logger.Log("Placed");
            _currentlyPlacingShadow.OnLeftParentShadow -= ForceDespawn;
            _currentlyPlacingShadow.ChageTriggerToCol();
            _shadow.PlaceNewShadow(_currentlyPlacingShadow);
            _currentlyPlacingShadow = null;
            return true;
        }
        else
        {
            Logger.Log("Can't place");
            return false;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ControllableShadow shadow = collision.attachedRigidbody.GetComponent<ControllableShadow>();
        if (shadow != null)
        {
            _shadow = shadow; Logger.Log($"{_shadow}  {Shadow}");
            _shadowBar.SetVisibility(true);
        }
        ITransmutableSadow shadow2 = collision.attachedRigidbody.GetComponent<ITransmutableSadow>();
        if(shadow2!=null)_transmutablkeShadow = shadow2;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        ControllableShadow shadow = collision.attachedRigidbody.GetComponent<ControllableShadow>();
        if (shadow != null && shadow == _shadow)
        {
            OnControllableShadowLeft?.Invoke();
            _shadowBar.SetVisibility(false);
            _shadow = null;
        }
        ITransmutableSadow shadow2 = collision.attachedRigidbody.GetComponent<ITransmutableSadow>();
        if (shadow2 != null && shadow2 == _transmutablkeShadow) _transmutablkeShadow = null;
    }
}
