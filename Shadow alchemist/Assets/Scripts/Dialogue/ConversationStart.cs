using DialogueEditor;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(NPCConversation))]
public class ConversationStart : MonoBehaviour
{
    [SerializeField] bool _useEvent;
    [SerializeField,ConditionalField(nameof(_useEvent))] TutorialStep _step;
    [SerializeField] NPCConversation conversation;
    [SerializeField] ConversationManager _convoMan;
    [SerializeField] PlayerDialogueInputHandler _dialogueInputHandler;
    [SerializeField] TMP_SpriteAsset _spriteAsset;
    private bool _conversationFinished;

    private void Awake()
    {
        if (_useEvent) _step.OnStepCompleted += StartConversation;     
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartConversation();
    }

    public void StartConversation()
    {
        if (_conversationFinished) return;

        if(_spriteAsset!=null) _convoMan.DialogueText.spriteAsset = _spriteAsset;
        _convoMan.OnConversationStarted += StopPlayerControls;
        _convoMan.OnConversationEnded += ConversationFinished;
        _convoMan.StartConversation(conversation);
        _dialogueInputHandler.StartConversation(_convoMan);
    }
    private void ConversationFinished()
    {
        _conversationFinished = true;
        _convoMan.OnConversationEnded -= ConversationFinished;
        _dialogueInputHandler.EndConversation();
    }
    private void StopPlayerControls()
    {
        _convoMan.OnConversationStarted -= StopPlayerControls;
    }
    private void OnValidate()
    {
        if ((conversation==null))
        {
            conversation = GetComponent<NPCConversation>();
        }
    }
    private void OnDestroy()
    {
        if(_useEvent) _step.OnStepCompleted -= StartConversation;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ConversationStart))]
public class ConversationStartEditor:Editor
{
    SerializedProperty _dialogueInput;
    private void OnEnable()
    {
        _dialogueInput = serializedObject.FindProperty("_dialogueInputHandler");
        serializedObject.Update();
       
        if(_dialogueInput.objectReferenceValue == null)
        {
            _dialogueInput.objectReferenceValue = FindFirstObjectByType<PlayerDialogueInputHandler>();
        }
        serializedObject.ApplyModifiedProperties();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif
