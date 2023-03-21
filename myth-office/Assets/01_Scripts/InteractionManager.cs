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

    public Prerequisites prerequisitesObject;
    public static List<string> allPrerequisites;
    public static List<string> fulfilledPrerequisites = new List<string>();

    private void OnValidate()
    {
        allPrerequisites = prerequisitesObject.prerequisites;
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
        AbstractInteractable abstractInteractable = other.gameObject.GetComponent<AbstractInteractable>();
        if (abstractInteractable == null)
        {
            return;
        }
        
        _interactables.Add(abstractInteractable);
    }

    private void OnTriggerExit(Collider other)
    {
        AbstractInteractable abstractInteractable = other.gameObject.GetComponent<AbstractInteractable>();
        if (abstractInteractable != null)
        {
            _interactables.Remove(abstractInteractable);
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
