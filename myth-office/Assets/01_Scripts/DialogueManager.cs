using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;

    private Canvas canvas;
    private TextMeshProUGUI textField;
    private Image leftImage;
    private Image rightImage;
    private AudioSource audioSource;
    
    private List<DialoguePart> dialogueParts;
    private int currentDialogue = 0;

    private void Start()
    {
        UI_REFS uiRefs = SceneManager.GetSceneByName("UI").GetRootGameObjects()[0].GetComponent<UI_REFS>();
        if (uiRefs)
        {
            canvas = uiRefs.canvas;
            textField = uiRefs.textField;
            leftImage = uiRefs.leftImage;
            rightImage = uiRefs.rightImage;
            audioSource = uiRefs.audioSource;
        }
        else
        {
            throw new Exception("UI References not found. Is UI_REFS the topmost object in the UI scene?");
        }

        dialogueParts = dialogue.dialogueParts;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        canvas.gameObject.SetActive(true);
        ProgressDialogue();
    }

    public void ReceiveInteractInput(InputAction.CallbackContext context)
    {
        bool isSuperfluousCallback = !context.started;
        if (isSuperfluousCallback)
        {
            return;
        }

        ProgressDialogue();
    }

    public void ProgressDialogue()
    {
        bool canvasNotEnabled = !canvas.gameObject.activeSelf;
        if (canvasNotEnabled)
        {
            return;
        }
        
        bool dialogueIndexIsInsideBounds = dialogueParts.Count > currentDialogue;
        if (dialogueIndexIsInsideBounds)
        { 
            textField.text = dialogueParts[currentDialogue].line; 
            leftImage.sprite = dialogueParts[currentDialogue].leftImage; 
            rightImage.sprite = dialogueParts[currentDialogue].rightImage; 
            audioSource.clip = dialogueParts[currentDialogue].sound; 
            audioSource.Play(); 
            currentDialogue++;
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        canvas.gameObject.SetActive(false);
        currentDialogue = 0;
    }
}
