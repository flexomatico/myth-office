using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class DialogueManager : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField][Range(0.1f, 10.0f)] private float colliderRadius;

    private EventSystem eventSystem;
    private Canvas canvas;
    private GameObject dialoguePanel;
    private TextMeshProUGUI textField;
    private Image leftImage;
    private Image rightImage;
    private AudioSource audioSource;
    private Button[] buttons;
    private TextMeshProUGUI leftName;
    private TextMeshProUGUI rightName;

    private List<DialoguePart> dialogueParts;
    private int currentDialogue = 0;

    private PlayerInput _playerInput;
    private SphereCollider _sphereCollider;

    void Awake()
    {
        _sphereCollider = gameObject.AddComponent<SphereCollider>();
        _sphereCollider.radius = colliderRadius;
        _sphereCollider.isTrigger = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }

    private void Start()
    {
        UI_REFS uiRefs = SceneManager.GetSceneByName("UI").GetRootGameObjects()[0].GetComponent<UI_REFS>();
        if (uiRefs)
        {
            eventSystem = uiRefs.eventSystem;
            canvas = uiRefs.canvas;
            dialoguePanel = uiRefs.dialoguePanel;
            textField = uiRefs.textField;
            leftImage = uiRefs.leftImage;
            rightImage = uiRefs.rightImage;
            audioSource = uiRefs.audioSource;
            buttons = uiRefs.buttons;
        }
        else
        {
            throw new Exception("UI References not found. Is UI_REFS the topmost object in the UI scene?");
        }
        
        dialogueParts = dialogue.dialogueParts;
    }

    public void StartInteraction(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        _playerInput.SwitchCurrentActionMap("UI");
        _playerInput.actions["Submit"].performed += ContinueInteraction;
        
        bool mainDialoguePanelIsHidden = !dialoguePanel.activeSelf;
        if(mainDialoguePanelIsHidden)
            dialoguePanel.SetActive(true);
        
        ContinueInteraction(new InputAction.CallbackContext());
    }
    
    public void ContinueInteraction(InputAction.CallbackContext context)
    {
        bool dialogueIndexIsOutsideBounds = dialogueParts.Count <= currentDialogue;
        if (dialogueIndexIsOutsideBounds)
        {
            EndInteraction();
            return;
        }
        
        ResetPlayerChoiceButtons();
        ResetDialogueTextField();
        
        switch (dialogueParts[currentDialogue].responseType)
        {
            case ResponseType.TextResponse:
                FillDialogueTextField();
                break;
            case ResponseType.ChoiceResponse:
                FillPlayerChoiceButtons();
                break;
        }
        
        FillDialogueAudioVisuals();
        currentDialogue++;
        print(currentDialogue);
    }

    public void EndInteraction()
    {
        dialoguePanel.gameObject.SetActive(false);
        _playerInput.actions["Submit"].performed -= ContinueInteraction;
        _playerInput.SwitchCurrentActionMap("Player");
        currentDialogue = 0;
    }

    private void ResetPlayerChoiceButtons()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void ResetDialogueTextField()
    {
        textField.gameObject.SetActive(false);
    }

    private void FillPlayerChoiceButtons()
    {
        ChoiceResponse response = dialogueParts[currentDialogue] as ChoiceResponse;
        for (int i = 0; i < response.choices.Length && i < buttons.Length; i++)
        {
            buttons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = response.choices[i];
            buttons[i].gameObject.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(buttons[0].gameObject);
    }

    private void FillDialogueTextField()
    {
        textField.gameObject.SetActive(true);
        TextResponse response = dialogueParts[currentDialogue] as TextResponse;
        textField.text = response.line;
    }

    private void FillDialogueAudioVisuals()
    {
        DialoguePart response = dialogueParts[currentDialogue];
        leftImage.sprite = response.leftImage;
        rightImage.sprite = response.rightImage;
        audioSource.clip = response.sound;
        audioSource.Play();
        leftName.text = response.speakerName;
        
    }
    
    // The onClick function of the buttons does not trigger anything.
    // Advancing the dialogue is solely handled by the InputManagers "Submit" Action.
    // This is due to problematic double click behaviour with mixing the two.
    /* 
    private void AttachDialogueContinueToButtons()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                ContinueInteraction(new InputAction.CallbackContext());
            });
        }
    }

    private void RemoveDialogueContinueFromButtons()
    {
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
    */
}
