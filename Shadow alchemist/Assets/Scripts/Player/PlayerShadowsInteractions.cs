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
    public bool FullShadow => _fullShadow;
    public ShadowBar ShadowBar =>_shadowBar;
    [SerializeField] float _shadowPlacingSpeed;
    [SerializeField] ShadowBar _shadowBar;
    [SerializeField] Transform _shadowSpawnPoint;
    [SerializeField] Transform _shadowSpawnPoint2;
    [SerializeField] bool _fullShadow;
    private PlacableShadow _currentlyPlacingShadow;
    private ITransmutableSadow _transmutablkeShadow;
    private float _shadowBarValue;
    private List<PlacableShadow> _placedShadows= new List<PlacableShadow>();
    private void Update()
    {
        if(Shadow==null) return;
        _shadowBar.SetValue(Shadow.TakenShadowBarValueFromTransmutation);
    }
    public void SpawnShadow(GameObject _shadowPrefab)
    {
        if(_fullShadow)
        {
            _currentlyPlacingShadow = Instantiate(_shadowPrefab, _shadowSpawnPoint.position, _shadowPrefab.transform.rotation, null).GetComponent<PlacableShadow>();
            _currentlyPlacingShadow.SetFullShadow();
        }
        else
        {
            _currentlyPlacingShadow = Instantiate(_shadowPrefab, Shadow.IsHorizontal ? _shadowSpawnPoint.position : _shadowSpawnPoint2.position, _shadowPrefab.transform.rotation, null).GetComponent<PlacableShadow>();
            _currentlyPlacingShadow.SetParentShadow(_shadow, _shadow.ShadowCollider);
        }

        Logger.Log($"{ _currentlyPlacingShadow}  {ShadowToPlace}");
    }
    public void DespawnShadow()
    {
        if(_currentlyPlacingShadow == null) return;
        Destroy(_currentlyPlacingShadow.gameObject);
        _currentlyPlacingShadow = null;
    }
    public bool PlaceShadow()
    {
        if (_currentlyPlacingShadow.CanBePlaced)
        {
            Logger.Log("Placed");
            _currentlyPlacingShadow.ChageTriggerToCol();
            if(!_fullShadow) _shadow.PlaceNewShadow(_currentlyPlacingShadow);
            else
            {
                if (_placedShadows.Count >= 3)
                {
                    _placedShadows[0].OnMaxTimeReached -= PlacedShadowTimeLimitreached;
                    Destroy(_placedShadows[0].gameObject);
                    _placedShadows.RemoveAt(0);

                }
                _currentlyPlacingShadow.StartTimeLimit();
                _currentlyPlacingShadow.OnMaxTimeReached += PlacedShadowTimeLimitreached;
                _placedShadows.Add(_currentlyPlacingShadow);
            }
            _currentlyPlacingShadow = null;
            return true;
        }
        else
        {
            return false;
        }
        
    }
    private void PlacedShadowTimeLimitreached(PlacableShadow shadow)
    {
        shadow.OnMaxTimeReached -= PlacedShadowTimeLimitreached;
        Destroy(shadow.gameObject);
        _placedShadows.Remove(shadow);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody)
        {
            ControllableShadow shadow = collision.attachedRigidbody.GetComponent<ControllableShadow>();
            if (shadow != null)
            {
                _shadow = shadow; Logger.Log($"{_shadow}  {Shadow}");
                _shadowBar.SetVisibility(true);
            }
            ITransmutableSadow shadow2 = collision.attachedRigidbody.GetComponent<ITransmutableSadow>();
            if (shadow2 != null) _transmutablkeShadow = shadow2;
        }

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
