using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConversationStartOnStart : ConversationStart
{
    [SerializeField] UnityEvent OnStart;
    [SerializeField] UnityEvent OnConvoEnd;
    // Start is called before the first frame update
    void Start()
    {

        Invoke("FireStart", 0.5f);
    }
    private void FireStart()
    {
        OnStart?.Invoke();
        _convoMan.OnConversationEnded += FireEndEvent;
        StartConversation();
    }
    private void FireEndEvent()
    {
        _convoMan.OnConversationEnded -= FireEndEvent;
        OnConvoEnd?.Invoke();
    }
    private void OnDestroy()
    {
        _convoMan.OnConversationEnded -= FireEndEvent;
    }
}
