using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakendStatus : MonoBehaviour
{
    public enum WeakenStatus
    {
        NONE,WEAKEN,STUNNED
    }
    public WeakenStatus Status;
    //public bool IsWeakened;
}
