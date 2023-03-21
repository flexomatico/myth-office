using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbstractInteractable : MonoBehaviour
{
    [ListToPopup(typeof(InteractionManager), "allPrerequisites")]
    public List<string> NeedsPrerequisites;
    [ListToPopup(typeof(InteractionManager), "allPrerequisites")]
    public List<string> FulfillsPrerequisites;
    
    public abstract void StartInteraction(PlayerInput playerInput);

    public abstract void ContinueInteraction(InputAction.CallbackContext context);

    public void EndInteraction()
    {
        InteractionManager.MarkPrerequisiteAsFulfilled(FulfillsPrerequisites);
    }
}
