using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(NPCConversation))]
public class ConversationStart : MonoBehaviour
{
    [SerializeField] NPCConversation conversation;
    [SerializeField] ConversationManager _convoMan;
    [SerializeField] PlayerDialogueInputHandler _dialogueInputHandler;
    private bool _conversationFinished;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartConversation();
    }

    public void StartConversation()
    {
        if (_conversationFinished) return;

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
