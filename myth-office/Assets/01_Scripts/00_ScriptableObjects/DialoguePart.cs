using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResponseType
{
    Invalid = -1,
    
    NPCResponse,
    PlayerResponse
}

[Serializable]
public abstract class DialoguePart
{
    [SerializeField, HideInInspector] public ResponseType responseType = ResponseType.Invalid;
}

[Serializable]
public class NPCResponse : DialoguePart
{
    [SerializeField, TextArea] public string line;
    [SerializeField] public Sprite leftImage;
    [SerializeField] public Sprite rightImage;
    [SerializeField] public AudioClip sound;

    public NPCResponse()
    {
        responseType = ResponseType.NPCResponse;
    }
}

[Serializable]
public class PlayerResponse : DialoguePart
{
    [SerializeField, TextArea] public string[] choices;

    public PlayerResponse()
    {
        responseType = ResponseType.PlayerResponse;
    }
}
