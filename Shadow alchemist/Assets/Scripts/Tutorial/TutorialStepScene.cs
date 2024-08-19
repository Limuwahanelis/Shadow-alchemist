using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStepScene : MonoBehaviour
{
    [SerializeField] TutorialStep _step;

    private void OnValidate()
    {
        if (_step == null) return;
        gameObject.name = _step.name;
    }
}
