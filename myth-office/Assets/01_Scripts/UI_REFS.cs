using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_REFS : MonoBehaviour
{
    public EventSystem eventSystem;
    public Canvas canvas;
    public GameObject mainDialoguePanel;
    public TextMeshProUGUI textField;
    public Image leftImage;
    public Image rightImage;
    public AudioSource audioSource;
    public GameObject playerChoicePanel;
    public Button[] buttons;
}
