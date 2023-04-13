using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class AbstractInteractable : MonoBehaviour
{
    [ListToPopup(typeof(InteractionManager), "allPrerequisites")]
    public List<string> NeedsPrerequisites;
    [ListToPopup(typeof(InteractionManager), "allPrerequisites")]
    public List<string> FulfillsPrerequisites;

    [SerializeField] protected bool deleteAfterFinished = false;

    [SerializeField] protected UnityEvent doAfterFinished;
    private bool hasInvokedAfterFinishedEvents = false;

    public abstract void SetInteractionPromptVisibility(bool state);
    
    public abstract void StartInteraction(PlayerInput playerInput);

    public abstract void ContinueInteraction(InputAction.CallbackContext context);

    public void EndInteraction()
    {
        if (!hasInvokedAfterFinishedEvents)
        {
            doAfterFinished.Invoke();
            hasInvokedAfterFinishedEvents = true;
        }
        InteractionManager.Instance.MarkPrerequisiteAsFulfilled(FulfillsPrerequisites);
        if (deleteAfterFinished)
        {
            InteractionManager.Instance.RemoveInteractable(this);
            Destroy(this);
        }
    }
}
