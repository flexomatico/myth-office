using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Footsteps : MonoBehaviour
{

    public AudioSource footstepsSound;
    public PlayerInput playerInput;
    void Update()
    {
        if (playerInput.currentActionMap.name != "Player")
        {
            return;
        }
        
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            footstepsSound.enabled = true; 
        }
        else 
        { 
            footstepsSound.enabled = false;
        }
    }

}
