using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public List<DialoguePart> dialogueParts;
}
