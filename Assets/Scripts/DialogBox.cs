using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    public Messages[] messagesSuper;
    public Actor[] actors;
    public DialogManager Box;

    public void StartDialogue(int dialogue)
    {
        Box.OpenDialogue(messagesSuper[dialogue].messages, actors);
    }
}

[System.Serializable]
public class Messages {
    public Message[] messages;
}

[System.Serializable]
public class Message {
    public int actorId;
    public string message;
}

[System.Serializable]
public class Actor {
    public string name;
}
