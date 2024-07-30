using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossCombat : MonoBehaviour,IDamager
{
    public enum AttackType
    {
        NORMAL_ATTACK,CHARGE
    }
    public ComboList ShadowFighterCombos => _comboList;

    public Vector3 Position => transform.position;

#if UNITY_EDITOR
    [SerializeField] bool _debug;
#endif

    [SerializeField] LayerMask _hitLayer;
    [SerializeField] Collider2D _col;

    [Header("Stun"), SerializeField] Sprite _stunSprite;
    [SerializeField] Sprite _normalSprite;
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Header("Attacks")]
    [SerializeField] ComboList _comboList;


    [Header("Attacks positions")]
    [SerializeField] Transform _normalAttack;
    [SerializeField] Transform _chargeAttack;

    [Header("Attacks sizes")]
    [SerializeField] Vector2 _normalAttackSize;
    [SerializeField] Vector2 _chargeAttackSize;


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
            case AttackType.NORMAL_ATTACK: hitEnemies = Physics2D.OverlapBoxAll(_normalAttack.position, _normalAttackSize, 0, _hitLayer).ToList(); break;
            case AttackType.CHARGE: hitEnemies = Physics2D.OverlapBoxAll(_normalAttack.position, _normalAttackSize, 0, _hitLayer).ToList(); break;
                //case AttackType.Fist1Attack2: hitEnemies = Physics2D.OverlapCircleAll(_chargeAttack.position, _chargeAttackSize, _hitLayer).ToList(); break;
        }


        int index = 0;
        for (; index < hitEnemies.Count; index++)
        {
            IPushable tmp2 = hitEnemies[index].GetComponentInParent<IPushable>();
            if (tmp2 != null) tmp2.Push(HealthSystem.DamageType.BOSS, this);
            IDamagable tmp = hitEnemies[index].GetComponentInParent<IDamagable>();
            if (tmp != null) tmp.TakeDamage(new DamageInfo(_comboList.comboList[((int)attackType)].Damage, PlayerHealthSystem.DamageType.ENEMY, transform.position));

        }
        yield return null;
        while (true)
        {
            Collider2D[] colliders =new Collider2D[0];
            switch (attackType)
            {
                case AttackType.NORMAL_ATTACK: colliders = Physics2D.OverlapBoxAll(_normalAttack.position, _normalAttackSize, 0, _hitLayer); break;
                case AttackType.CHARGE: colliders = Physics2D.OverlapBoxAll(_normalAttack.position, _normalAttackSize, 0, _hitLayer); break;
               // case AttackType.Fist2Attack1: colliders = Physics2D.OverlapCircleAll(_fistAttack2Pos.position, _fistAttack2Size, 0, _hitLayer); break;
            }
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!hitEnemies.Contains(colliders[i]))
                {
                    hitEnemies.Add(colliders[i]);
                    IPushable tmp2 = hitEnemies[index].GetComponentInParent<IPushable>();
                    if (tmp2 != null) tmp2.Push( HealthSystem.DamageType.BOSS, this);
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
            if (_normalAttack != null) Gizmos.DrawWireCube(_normalAttack.position, _normalAttackSize);
            //if (_chargeAttack) Gizmos.DrawWireSphere(_chargeAttack.position, _chargeAttackSize);
            //if (_fistAttack2Pos) Gizmos.DrawWireSphere(_fistAttack2Pos.position, _fistAttack2Size);
        }
    }
#endif
    public void ResumeCollisons(Collider2D[] playerCols)
    {
        foreach (Collider2D col in playerCols)
        {
            Physics2D.IgnoreCollision(col, _col,false);
        }
    }

    public void PreventCollisions(Collider2D[] playerCols)
    {
        foreach (Collider2D col in playerCols)
        {
            Physics2D.IgnoreCollision(col, _col,true);
        }
    }
}
