using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour
{
    public Animator animator;
    public Light[] dirLights;

    // Start is called before the first frame update
    public void FadeInDirectionalLights()
    {
        animator.enabled = true;
        animator.SetTrigger("fade-in");
    }

    public void TurnOffDirectionalLights()
    {
        animator.enabled = false;
        foreach (Light light in dirLights)
        {
            light.intensity = 0;
        }
    }
    
}
