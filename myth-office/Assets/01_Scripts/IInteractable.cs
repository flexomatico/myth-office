using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void StartInteraction(PlayerInput playerInput);
    void ContinueInteraction(InputAction.CallbackContext context);
}
