using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputHandlerTutorial;

[CreateAssetMenu( fileName ="new tutorial step",menuName ="Tutorial step")]
public class TutorialStep : ScriptableObject
{
    public TutorialStepEn Step;
    public Action OnStepCompleted;
}
