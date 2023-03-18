using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupTest : MonoBehaviour, ISerializationCallbackReceiver
{
        
    public static List<string> TMPList;
    [HideInInspector] public List<string> popupList;
    [ListToPopup(typeof(PopupTest), "TMPList")]
    public List<string> Prerequisites;

    [ContextMenu("Create List")]
    private void CreateList()
    {
        popupList = new List<string> { "Talked to Zeus", "Picked up paper", "Finished Level" };
    }

    public void OnBeforeSerialize()
    {
        TMPList = popupList;
    }
    
    public void OnAfterDeserialize() {}
}
