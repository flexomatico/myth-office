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
    public GameObject dialoguePanel;
    public VerticalLayoutGroup dialogueLayoutGroup;
    public TextMeshProUGUI textField;
    public Image leftImage;
    public Image middleImage;
    public Image rightImage;
    public AudioSource audioSource;
    public Button[] buttons;
    public TextMeshProUGUI leftName;
    public TextMeshProUGUI rightName;
}
