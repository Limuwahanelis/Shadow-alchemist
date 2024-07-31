using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSpriteMaskBoss : MonoBehaviour
{
    [SerializeField] Sprite _lanceTipUp;
    [SerializeField] Sprite _lanceTipDown;
    [SerializeField] SpriteMask _mask;

    public void SetLanceTipUp()
    {
        _mask.sprite = _lanceTipUp;
    }
    public void SetLanceTipDown()
    {
        _mask.sprite = _lanceTipDown;
    }
}
