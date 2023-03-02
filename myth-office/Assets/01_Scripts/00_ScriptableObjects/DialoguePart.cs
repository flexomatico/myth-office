using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialoguePart
{
    [TextArea(3, 10)] [SerializeField] public string line;
    [SerializeField] public Sprite leftImage;
    [SerializeField] public Sprite rightImage;
    [SerializeField] public AudioClip sound;
}
