using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFighterCombat : MonoBehaviour
{
    public enum AttackType
    {
        SLASH, STAB
    }
    //public ComboList SkeletonCombos => _comboList;
    public float[] AttacksDelays => _attacksDelays;

#if UNITY_EDITOR
    [SerializeField] bool _debug;
#endif

    [SerializeField] LayerMask _hitLayer;

    //[Header("Attacks")]
    //[SerializeField] ComboList _comboList;

    [Header("Attacks delays")]
    [SerializeField] float[] _attacksDelays;

    [Header("Attacks positions")]
    [SerializeField] Transform _slashAttackPos;
    [SerializeField] Transform _stabAttackPos;

    [Header("Attacks sizes")]
    [SerializeField] Vector2 _slashAttackSize;
    [SerializeField] Vector2 _stabAttackSize;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
