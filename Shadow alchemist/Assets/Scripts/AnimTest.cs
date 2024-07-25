using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimTest : MonoBehaviour
{
    public string[] animationTag;
    public GameObject playerGO;
    private void OnAnimatorMove()
    {
        Animator anim = gameObject.GetComponent<Animator>();

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        playerGO.transform.position += anim.deltaPosition;
        for (int i = 0; i < animationTag.Length; i++)
        {
            if (stateInfo.IsTag(animationTag[i]))
            {
                anim.applyRootMotion = true;
                //anim.ApplyBuiltinRootMotion();
            }
            else anim.applyRootMotion = false;
        }
    }
}