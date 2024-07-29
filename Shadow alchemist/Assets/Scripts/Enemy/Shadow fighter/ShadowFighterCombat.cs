using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ShadowFighterCombat : MonoBehaviour,IDamager
{
    public enum AttackType
    {
        Fist1Attack1,Fist1Attack2,Fist2Attack1
    }
    public ComboList ShadowFighterCombos => _comboList;
    //public ComboList SkeletonCombos => _comboList;
    public float[] AttacksDelays => _attacksDelays;

    public Vector3 Position => transform.position;

#if UNITY_EDITOR
    [SerializeField] bool _debug;
#endif

    [SerializeField] LayerMask _hitLayer;

    [Header("Stun"), SerializeField] Sprite _stunSprite;
    [SerializeField] Sprite _normalSprite;
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Header("Attacks")]
    [SerializeField] ComboList _comboList;


    [Header("Attacks positions")]
    [SerializeField] Transform _fistAttack1Pos;
    [SerializeField] Transform _fistAttack1ExtendedPos;
    [SerializeField] Transform _fistAttack2Pos;

    [Header("Attacks sizes")]
    [SerializeField] Vector2 _fistAttack1Size;
    [SerializeField] float _fistAttack1ExtendedSize;
    [SerializeField] float _fistAttack2Size;


    [Header("Attacks delays")]
    [SerializeField] float[] _attacksDelays;
    public void SetStunViusals(bool value)
    {
        if (value) _spriteRenderer.sprite = _stunSprite;
        else _spriteRenderer.sprite = _normalSprite;
    }
    public IEnumerator AttackCor(AttackType attackType)
    {
        List<Collider2D> hitEnemies = new List<Collider2D>();
        switch (attackType)
        {
            case AttackType.Fist1Attack1: hitEnemies = Physics2D.OverlapBoxAll(_fistAttack1Pos.position, _fistAttack1Size, 0, _hitLayer).ToList(); break;
            case AttackType.Fist1Attack2: hitEnemies = Physics2D.OverlapCircleAll(_fistAttack1ExtendedPos.position, _fistAttack1ExtendedSize, _hitLayer).ToList(); break;
            case AttackType.Fist2Attack1: hitEnemies = Physics2D.OverlapCircleAll(_fistAttack2Pos.position, _fistAttack2Size, _hitLayer).ToList(); break;
        }


        int index = 0;
        for (; index < hitEnemies.Count; index++)
        {
            IPushable tmp2 = hitEnemies[index].GetComponentInParent<IPushable>();
            if (tmp2 != null) tmp2.Push(HealthSystem.DamageType.ENEMY,this);
            IDamagable tmp = hitEnemies[index].GetComponentInParent<IDamagable>();
            if (tmp != null) tmp.TakeDamage(new DamageInfo(_comboList.comboList[((int)attackType)].Damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));

        }
        yield return null;
        while (true)
        {
            Collider2D[] colliders = null;
            switch (attackType)
            {
                case AttackType.Fist1Attack1: colliders = Physics2D.OverlapBoxAll(_fistAttack1Pos.position, _fistAttack1Size, 0, _hitLayer); break;
                case AttackType.Fist1Attack2: colliders = Physics2D.OverlapCircleAll(_fistAttack1ExtendedPos.position, _fistAttack1ExtendedSize, 0, _hitLayer); break;
                case AttackType.Fist2Attack1: colliders = Physics2D.OverlapCircleAll(_fistAttack2Pos.position, _fistAttack2Size, 0, _hitLayer); break;
            }
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!hitEnemies.Contains(colliders[i]))
                {
                    hitEnemies.Add(colliders[i]);
                    IPushable tmp2 = hitEnemies[index].GetComponentInParent<IPushable>();
                    if (tmp2 != null) tmp2.Push(Vector2.zero, HealthSystem.DamageType.ENEMY, this);
                    IDamagable tmp = colliders[i].GetComponentInParent<IDamagable>();
                    if (tmp != null) tmp.TakeDamage(new DamageInfo(_comboList.comboList[((int)attackType)].Damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));

                }
            }
            yield return null;
        }
    }


#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (_debug)
        {
            Gizmos.color = Color.white;
            if (_fistAttack1Pos != null) Gizmos.DrawWireCube(_fistAttack1Pos.position, _fistAttack1Size);
            if (_fistAttack1ExtendedPos) Gizmos.DrawWireSphere(_fistAttack1ExtendedPos.position, _fistAttack1ExtendedSize);
            if (_fistAttack2Pos) Gizmos.DrawWireSphere(_fistAttack2Pos.position, _fistAttack2Size);
        }
    }
#endif
    public void ResumeCollisons(Collider2D[] playerCols)
    {
        
    }

    public void PreventCollisions(Collider2D[] playerCols)
    {
       
    }

}
