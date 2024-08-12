using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowControlModeSelectionUITutorial : ShadowControlModeSelectionUI
{
    public UnityEvent OnShadowControlModeOnteredFirstTime;
    public UnityEvent OnShadowControlModeOnteredSecondTime;
    public UnityEvent OnShadowTransmutationModeEnteredFirstTime;
    private int _shadwoControlEnterTime = 1;
    private bool _firstTimeShadowControl = true;
    private bool _dirstTimeShadowtransmutation = true;
    public override void SetVisiblity(bool visiblity)
    {
        if(visiblity)
        {
            if(_shadwoControlEnterTime==1)
            {
                _shadwoControlEnterTime++;
                OnShadowControlModeOnteredFirstTime?.Invoke();
            }
            else if(_shadwoControlEnterTime==2)
            {
                OnShadowControlModeOnteredSecondTime?.Invoke();
                _shadwoControlEnterTime++;
            }
        }
        base.SetVisiblity(visiblity);
    }
    public override void SelectShadowTransmutation()
    {
        if(_dirstTimeShadowtransmutation)
        {
            _dirstTimeShadowtransmutation = false;
            OnShadowTransmutationModeEnteredFirstTime?.Invoke();
        }
        base.SelectShadowTransmutation();
    }
}
