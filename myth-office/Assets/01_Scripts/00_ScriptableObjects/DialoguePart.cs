using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueType
{
    Invalid = -1,
    
    NPCResponse,
    PlayerResponse
}

[Serializable]
public abstract class DialoguePart
{
    [SerializeField, HideInInspector] protected DialogueType dialogueType = DialogueType.Invalid;
}

[Serializable]
public class NPCResponse : DialoguePart
{
    [SerializeField] public string line;
    [SerializeField] public Sprite leftImage;
    [SerializeField] public Sprite rightImage;
    [SerializeField] public AudioClip sound;

    public NPCResponse()
    {
        dialogueType = DialogueType.NPCResponse;
    }
}

[Serializable]
public class PlayerResponse : DialoguePart
{
    [SerializeField] public List<string> choices;

    public PlayerResponse()
    {
        dialogueType = DialogueType.PlayerResponse;
    }
}
