using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDebugCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }
    
}
