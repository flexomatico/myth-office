using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField][Range(0.1f, 10.0f)] private float colliderRadius;
    
    private Canvas canvas;
    private TextMeshProUGUI textField;
    private Image leftImage;
    private Image rightImage;
    private AudioSource audioSource;
    
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

    public void StartInteraction(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        canvas.gameObject.SetActive(true);
        ContinueInteraction(new InputAction.CallbackContext());
    }
    
    public void ContinueInteraction(InputAction.CallbackContext context)
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
        _playerInput.actions["Submit"].performed -= ContinueInteraction;
        _playerInput.SwitchCurrentActionMap("Player");
    }
}
