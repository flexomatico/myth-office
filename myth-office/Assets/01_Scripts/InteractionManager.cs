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
    private PlayerInput _playerInput;

    public Prerequisites prerequisites;
    public static List<string> allPrerequisites;
    public static List<string> fulfilledPrerequisites = new List<string>();
    [ListToPopup(typeof(InteractionManager), "allPrerequisites")]
    public List<string> InitiallyFulfilledPrerequisites;

    private void OnValidate()
    {
        allPrerequisites = prerequisites.prerequisites;
        fulfilledPrerequisites = InitiallyFulfilledPrerequisites;
    }

    public static void MarkPrerequisiteAsFulfilled(List<string> fulfilled)
    {
        foreach (string s in fulfilled)
        {
            bool isAlreadyFulfilled = fulfilled.Contains(s);
            if (isAlreadyFulfilled)
                continue;

            fulfilledPrerequisites.Add(s);
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
            _interactables.Add(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        AbstractInteractable interactable = other.gameObject.GetComponent<AbstractInteractable>();
        if (interactable != null)
        {
            _interactables.Remove(interactable);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        bool noInteractablesAround = _interactables.Count <= 0;
        if (noInteractablesAround)
        {
            return;
        }
        
        _interactables[0].StartInteraction(_playerInput);
    }
}
