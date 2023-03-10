using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    private List<IInteractable> _interactables = new List<IInteractable>();

    private CapsuleCollider _collider;
    private PlayerInput _playerInput;

    void Start()
    {
        _collider = gameObject.GetComponent<CapsuleCollider>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Interact"].performed += Interact;
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            _interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            _interactables.Remove(interactable);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        bool listContainsItems = _interactables.Count > 0;
        if (listContainsItems)
        {
            print("Interaction Manager triggered");
            _interactables[0].StartInteraction(_playerInput);
            //_playerInput.actions["Submit"].performed += _interactables[0].ContinueInteraction;
            //_playerInput.SwitchCurrentActionMap("UI");
        }
    }
}
