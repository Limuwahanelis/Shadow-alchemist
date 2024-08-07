using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
public class ConvoTest : MonoBehaviour
{
    [SerializeField] NPCConversation conversation;
    [SerializeField] ConversationManager _convoMan;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _convoMan.StartConversation(conversation);
    }
}
