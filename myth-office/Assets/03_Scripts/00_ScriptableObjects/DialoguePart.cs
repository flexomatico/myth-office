using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResponseType
{
    Invalid = -1,
    
    TextResponse,
    ChoiceResponse
}

[Serializable]
public abstract class DialoguePart
{
    [SerializeField, HideInInspector] public ResponseType responseType = ResponseType.Invalid;
    [SerializeField] public string speakerName;
    [SerializeField, HideInInspector] public int speakerLocation = 0;
    [SerializeField] public Sprite leftImage;
    [SerializeField] public Sprite middleImage;
    [SerializeField] public Sprite rightImage;
    [SerializeField] public AudioClip sound;
}

[Serializable]
public class TextResponse : DialoguePart
{
    [SerializeField, TextArea] public string line;

    public TextResponse()
    {
        responseType = ResponseType.TextResponse;
    }
}

[Serializable]
public class ChoiceResponse : DialoguePart
{
    [SerializeField, TextArea] public string[] choices;

    public ChoiceResponse()
    {
        responseType = ResponseType.ChoiceResponse;
    }
}
