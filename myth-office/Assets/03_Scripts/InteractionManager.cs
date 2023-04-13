using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    private List<AbstractInteractable> _interactables = new List<AbstractInteractable>();

    private CapsuleCollider _collider;
    public PlayerInput _playerInput;

    public Prerequisites prerequisites;
    public static List<string> allPrerequisites;
    public static List<string> fulfilledPrerequisites = new List<string>();
    [ListToPopup(typeof(InteractionManager), "allPrerequisites")]
    public List<string> InitiallyFulfilledPrerequisites;

    public static InteractionManager Instance { get; private set; }

    private void OnValidate()
    {
        allPrerequisites = prerequisites.prerequisites;
        fulfilledPrerequisites = InitiallyFulfilledPrerequisites;
    }

    public void MarkPrerequisiteAsFulfilled(List<string> fulfills)
    {
        foreach (string s in fulfills)
        {
            bool isAlreadyFulfilled = fulfilledPrerequisites.Contains(s);
            if (isAlreadyFulfilled)
                continue;

            fulfilledPrerequisites.Add(s);
        }
    }

    public void RemoveInteractable(AbstractInteractable interactable)
    {
        _interactables.Remove(interactable);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        _collider = gameObject.GetComponent<CapsuleCollider>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Interact"].performed += Interact;
    }

    private void OnTriggerEnter(Collider other)
    {
        AbstractInteractable interactable = other.gameObject.GetComponent<AbstractInteractable>();
        if (interactable == null)
        {
            return;
        }

        bool isNotInListYet = !_interactables.Contains(interactable);
        if (isNotInListYet)
        {
            _interactables.Add(interactable);
            interactable.ToggleInteractionPromptVisibility();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AbstractInteractable interactable = other.gameObject.GetComponent<AbstractInteractable>();
        if (interactable != null)
        {
            _interactables.Remove(interactable);
            interactable.ToggleInteractionPromptVisibility();
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        bool noInteractablesAround = _interactables.Count <= 0;
        if (noInteractablesAround)
        {
            return;
        }

        foreach (AbstractInteractable interactable in _interactables)
        {
            bool fulfillsAllPrerequisites = true;
            foreach (string s in interactable.NeedsPrerequisites)
            {
                bool prerequisiteNotFulfilled = !fulfilledPrerequisites.Contains(s);
                if (prerequisiteNotFulfilled)
                {
                    fulfillsAllPrerequisites = false;
                }
            }

            if (fulfillsAllPrerequisites)
            {
                interactable.StartInteraction(_playerInput);
                return;
            }
        }

    }
}
