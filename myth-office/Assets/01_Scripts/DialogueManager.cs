using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class DialogueManager : AbstractInteractable
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
        _sphereCollider.radius = colliderRadius / transform.localScale.x;
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
            leftName = uiRefs.leftName;
            rightName = uiRefs.rightName;
        }
        else
        {
            throw new Exception("UI References not found. Is UI_REFS the topmost object in the UI scene?");
        }
        
        dialogueParts = dialogue.dialogueParts;
    }

    public override void StartInteraction(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        _playerInput.SwitchCurrentActionMap("UI");
        _playerInput.actions["Submit"].performed += ContinueInteraction;
        
        bool mainDialoguePanelIsHidden = !dialoguePanel.activeSelf;
        if(mainDialoguePanelIsHidden)
            dialoguePanel.SetActive(true);
        
        ResetNameTextFields();
        ContinueInteraction(new InputAction.CallbackContext());
    }
    
    public override void ContinueInteraction(InputAction.CallbackContext context)
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
        FillDialogueNames();
        SetActiveSpeaker();
        currentDialogue++;
    }

    public new void EndInteraction()
    {
        base.EndInteraction();
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
        if (response.leftImage != null)
        {
            leftImage.sprite = response.leftImage;
            leftImage.preserveAspect = true;
            SetSpeakerImageRect(response.leftImage, leftImage);
        }

        if (response.rightImage != null)
        {
            rightImage.sprite = response.rightImage;
            rightImage.preserveAspect = true;
            SetSpeakerImageRect(response.rightImage, rightImage);
        }
        
        audioSource.clip = response.sound;
        audioSource.Play();
    }

    private void SetSpeakerImageRect(Sprite sprite, Image imageComponent)
    {
        bool widthGreaterThanHeight = sprite.texture.width > sprite.texture.height;
        if (widthGreaterThanHeight)
        {
            imageComponent.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else
        {
            imageComponent.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }

    private void FillDialogueNames()
    {
        DialoguePart response = dialogueParts[currentDialogue];
        switch (response.speakerLocation)
        {
            case 0:
                leftName.gameObject.SetActive(true);
                leftName.text = response.speakerName;
                rightName.gameObject.SetActive(false);
                break;
            case 1:
                rightName.gameObject.SetActive(true);
                rightName.text = response.speakerName;
                leftName.gameObject.SetActive(false);
                break;
        }
    }

    private void ResetNameTextFields()
    {
        leftName.text = "...";
        rightName.text = "...";
    }

    private void SetActiveSpeaker()
    {
        DialoguePart response = dialogueParts[currentDialogue];
        switch (response.speakerLocation)
        {
            case 0:
                leftImage.color = Color.white;
                rightImage.color = Color.grey;
                break;
            case 1:
                rightImage.color = Color.white;
                leftImage.color = Color.grey;
                break;
        }
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
