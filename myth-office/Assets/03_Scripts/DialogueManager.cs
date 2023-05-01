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
    [SerializeField] public Dialogue dialogue;
    [SerializeField][Range(0.1f, 10.0f)] private float colliderRadius = 3.0f;

    private EventSystem eventSystem;
    private Canvas canvas;
    private GameObject dialoguePanel;
    private VerticalLayoutGroup dialoguelayoutGroup;
    private TextMeshProUGUI textField;
    private Image leftImage;
    private Image middleImage;
    private Image rightImage;
    private AudioSource audioSource;
    private Button[] buttons;
    private TextMeshProUGUI leftName;
    private TextMeshProUGUI rightName;

    private List<DialoguePart> dialogueParts;
    private int currentDialogue = 0;

    private PlayerInput _playerInput;
    private SphereCollider _sphereCollider;

    private GameObject interactionPrompt;

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
        //UI_REFS uiRefs = SceneManager.GetSceneByName("UI").GetRootGameObjects()[0].GetComponent<UI_REFS>();
        UI_REFS uiRefs = FindObjectOfType<UI_REFS>();
        if (uiRefs)
        {
            eventSystem = uiRefs.eventSystem;
            canvas = uiRefs.canvas;
            dialoguePanel = uiRefs.dialoguePanel;
            dialoguelayoutGroup = uiRefs.dialogueLayoutGroup;
            textField = uiRefs.textField;
            leftImage = uiRefs.leftImage;
            middleImage = uiRefs.middleImage;
            rightImage = uiRefs.rightImage;
            audioSource = uiRefs.audioSource;
            buttons = uiRefs.buttons;
            leftName = uiRefs.leftName;
            rightName = uiRefs.rightName;
            interactionPrompt = uiRefs.interactionPrompt;
        }
        else
        {
            throw new Exception("UI References not found. Is UI_REFS the topmost object in the UI scene?");
        }
        
        dialogueParts = dialogue.dialogueParts;

        CreateInteractionPrompt();
    }

    private void CreateInteractionPrompt()
    {
        interactionPrompt = Instantiate(interactionPrompt, transform);
        float scale = interactionPrompt.transform.localScale.x / transform.localScale.x;
        interactionPrompt.transform.localScale = new Vector3(scale, scale, scale);
        SetInteractionPromptVisibility(false);
    }

    public override void SetInteractionPromptVisibility(bool state)
    {
        interactionPrompt.SetActive(state);
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
                
                // If we don't force update the text will be displaced until the frame after the next change in UI.
                LayoutRebuilder.ForceRebuildLayoutImmediate(dialoguelayoutGroup.GetComponent<RectTransform>());
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
        ResetSpeakerImages();
        dialoguePanel.gameObject.SetActive(false);
        InteractionManager.Instance._playerInput.actions["Submit"].performed -= ContinueInteraction;
        InteractionManager.Instance._playerInput.SwitchCurrentActionMap("Player");
        currentDialogue = 0;

        if (deleteAfterFinished)
        {
            Destroy(_sphereCollider);
            Destroy(interactionPrompt);
        }
        base.EndInteraction();
    }

    public void DeleteDialogueManager()
    {
        deleteAfterFinished = true;
        EndInteraction();
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
            leftImage.enabled = true;
            leftImage.sprite = response.leftImage;
            leftImage.preserveAspect = true;
            SetSpeakerImageRect(response.leftImage, leftImage);
            leftImage.SetNativeSize();

            // Flip images on the left side of the screen so that they look towards the center.
            // leftImage.gameObject.transform.localScale = new Vector3(-1, 1, 1);


            // Allow each image to have a custom pivot by reading pivots from the sprite data.
            Texture2D leftTexture = leftImage.sprite.texture;
            leftImage.GetComponent<RectTransform>().pivot = leftImage.sprite.pivot / new Vector2(leftTexture.width, leftTexture.height);
        }
        else if (leftImage.sprite == null)
        {
            leftImage.enabled = false;
        }

        if (response.rightImage != null)
        {
            rightImage.enabled = true;
            rightImage.sprite = response.rightImage;
            rightImage.preserveAspect = true;
            SetSpeakerImageRect(response.rightImage, rightImage);
            rightImage.SetNativeSize();

            // Allow each image to have a custom pivot by reading pivots from the sprite data.
            Texture2D rightTexture = rightImage.sprite.texture;
            rightImage.GetComponent<RectTransform>().pivot = rightImage.sprite.pivot / new Vector2(rightTexture.width, rightTexture.height);
        }
        else if (rightImage.sprite == null)
        {
            rightImage.enabled = false;
        }

        if (response.middleImage != null)
        {
            middleImage.sprite = response.middleImage;
            middleImage.enabled = true;
        }
        else
        {
            middleImage.enabled = false;
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
                // Enable the current speaking name
                leftName.transform.parent.gameObject.SetActive(true);
                leftName.text = response.speakerName;

                // Disable the name of the side that isn't currently speaking.
                rightName.transform.parent.gameObject.SetActive(false);
                break;
            case 1:
                // Enable the current speaking name
                rightName.transform.parent.gameObject.SetActive(true);
                rightName.text = response.speakerName;

                // Disable the name of the side that isn't currently speaking.
                leftName.transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    private void ResetNameTextFields()
    {
        leftName.text = "...";
        rightName.text = "...";
    }

    private void ResetSpeakerImages()
    {
        leftImage.sprite = null;
        middleImage.sprite = null;
        rightImage.sprite = null;
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
