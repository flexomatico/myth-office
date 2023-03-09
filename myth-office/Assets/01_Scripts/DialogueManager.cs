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
    private GameObject mainDialoguePanel;
    private TextMeshProUGUI textField;
    private Image leftImage;
    private Image rightImage;
    private AudioSource audioSource;

    private GameObject playerChoicePanel;
    private Button[] buttons;

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
            mainDialoguePanel = uiRefs.mainDialoguePanel;
            textField = uiRefs.textField;
            leftImage = uiRefs.leftImage;
            rightImage = uiRefs.rightImage;
            audioSource = uiRefs.audioSource;
            playerChoicePanel = uiRefs.playerChoicePanel;
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
        Invoke("AttachDialogueContinueToButtons", 0.1f); // If the onClick method of the buttons is attached immediately, they will fire immediately
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
        
        switch (dialogueParts[currentDialogue].responseType)
        {
            case ResponseType.NPCResponse:
                FillMainDialogueField();
                PrepareForNextDialoguePartFromNPC();
                break;
            case ResponseType.PlayerResponse:
                FillPlayerChoiceButtons();
                PrepareForNextDialoguePartFromPlayerChoice();
                currentDialogue++;
                break;
        }
    }

    public void EndInteraction()
    {
        mainDialoguePanel.gameObject.SetActive(false);
        playerChoicePanel.gameObject.SetActive(false);
        RemoveDialogueContinueFromButtons();
        _playerInput.SwitchCurrentActionMap("Player");
        currentDialogue = 0;
    }

    private void AttachDialogueContinueToButtons()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => ContinueInteraction(new InputAction.CallbackContext()));
        }
    }

    private void RemoveDialogueContinueFromButtons()
    {
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void ResetPlayerChoiceButtons()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void FillPlayerChoiceButtons()
    {
        bool playerChoicePanelIsHidden = !playerChoicePanel.activeSelf;
        if(playerChoicePanelIsHidden)
            playerChoicePanel.SetActive(true);
        
        PlayerResponse response = dialogueParts[currentDialogue] as PlayerResponse;
        for (int i = 0; i < response.choices.Length && i < buttons.Length; i++)
        {
            buttons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = response.choices[i];
            buttons[i].gameObject.SetActive(true);
        }
        eventSystem.SetSelectedGameObject(buttons[0].gameObject);
    }

    private void FillMainDialogueField()
    {
        bool mainDialoguePanelIsHidden = !mainDialoguePanel.activeSelf;
        if(mainDialoguePanelIsHidden)
            mainDialoguePanel.SetActive(true);
        
        NPCResponse response = dialogueParts[currentDialogue] as NPCResponse;
        textField.text = response.line;
        leftImage.sprite = response.leftImage;
        rightImage.sprite = response.rightImage;
        audioSource.clip = response.sound;
        audioSource.Play();
    }

    private void PrepareForNextDialoguePartFromNPC()
    {
        bool isLastDialoguePart = currentDialogue == dialogueParts.Count - 1;
        if (isLastDialoguePart)
        {
            _playerInput.actions["Submit"].performed += ContinueInteraction;
            currentDialogue++;
            return;
        }
        
        ResponseType nextResponseType = dialogueParts[currentDialogue + 1].responseType;
        currentDialogue++;
        switch(nextResponseType) {
            case ResponseType.PlayerResponse:
                _playerInput.actions["Submit"].performed -= ContinueInteraction;
                FillPlayerChoiceButtons();
                break;
            case ResponseType.NPCResponse:
                _playerInput.actions["Submit"].performed += ContinueInteraction;
                break;
        }
    }

    private void PrepareForNextDialoguePartFromPlayerChoice()
    {
        bool isFirstDialoguePart = currentDialogue == 0;
        if (isFirstDialoguePart)
        {
           return;
        }
        
        bool isLastDialoguePart = currentDialogue == dialogueParts.Count - 1;
        ResponseType previousResponseType = dialogueParts[currentDialogue - 1].responseType;
        if (isLastDialoguePart || previousResponseType == ResponseType.PlayerResponse)
        {
            textField.text = "...";
        }
    }
}
