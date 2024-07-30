using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChargeInfo : MonoBehaviour
{
    public bool ShouldStartCharge => _shouldStartCharge;
    private bool _shouldStartCharge;

    public void StartCharge()
    {
        _shouldStartCharge = true;
    }
    public void ResetCharge()
    {
        _shouldStartCharge=false;
    }
}
