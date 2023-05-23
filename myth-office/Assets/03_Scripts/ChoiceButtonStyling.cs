using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceButtonStyling : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color defaultColor = Color.black;
    public Color selectedColor = new Color(34, 54, 131);
    public float characterSpacingWhenBold = -7.0f;

    private TextMeshProUGUI textComp;
    private string text;

    private void Awake()
    {
        textComp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text = textComp.text;
    }

    public void OnSelect(BaseEventData eventData)
    {
        textComp.fontStyle = FontStyles.Bold;
        textComp.characterSpacing = characterSpacingWhenBold;
        textComp.color = selectedColor;
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        textComp.fontStyle = FontStyles.Normal;
        textComp.characterSpacing = 0;
        textComp.color = defaultColor;
    }
    
    // When highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }
}
