using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{

    public Animator cameraAnimator;
    public Animator creditsAnimator;

    public void playCredits()
    {
        cameraAnimator.SetTrigger("move-down");
        creditsAnimator.SetTrigger("start-scrolling");
    }
}
