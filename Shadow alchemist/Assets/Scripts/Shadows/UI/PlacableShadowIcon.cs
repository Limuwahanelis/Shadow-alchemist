using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlacableShadowIcon : MonoBehaviour
{
    public UnityEvent<GameObject> OnShadowPicked;
    public GameObject ShadowPrefab => _placableShadowPrefab;
    [SerializeField] GameObject _placableShadowPrefab;
    [SerializeField] GameObject _highlight;

    public void DeselectIcon()
    {
        _highlight.SetActive(false);
    }
    public void SelectIcon()
    {
        _highlight.SetActive(true);
    }
    public void SelectShadow()
    {
        OnShadowPicked?.Invoke(_placableShadowPrefab);
    }
}
