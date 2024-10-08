using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTMPSpriteAsset : MonoBehaviour
{
    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    public void SetSpriteAsset(TMP_SpriteAsset newSpriteAsset)
    {
        _text.spriteAsset = newSpriteAsset;
    }
}
