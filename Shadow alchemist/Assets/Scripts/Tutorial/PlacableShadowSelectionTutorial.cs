using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacableShadowSelectionTutorial : PlacableShadowSelection
{
    public UnityEvent OnShadowFirstSelected;
    public override void SelectShadow()
    {
        OnShadowFirstSelected.Invoke();
        base.SelectShadow();
    }
}
