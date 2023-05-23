using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public EventSystem eventSystem;
    public PlayerInput _playerInput;
    public GameObject playButton;
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public Animator cameraAnimator;
    public Animator gradientAnimator;
    
    private bool hasStarted = false;
    private string lastActionMap;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem.SetSelectedGameObject(playButton);
        _playerInput.SwitchCurrentActionMap("Menu");
        SetCursorLock(false);
    }

    private void SetCursorLock(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void StartGame()
    {
        _playerInput.SwitchCurrentActionMap("Player");
        startMenu.SetActive(false);
        cameraAnimator.SetTrigger("continue-camera");
        gradientAnimator.SetTrigger("fade-out");
        hasStarted = true;
        SetCursorLock(true);
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            lastActionMap = _playerInput.currentActionMap.name;
        }
        _playerInput.SwitchCurrentActionMap("Menu");
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);
        cameraAnimator.SetTrigger("pause-camera");
        gradientAnimator.SetTrigger("fade-in");
        SetCursorLock(false);
    }

    public void ContinueGame()
    {
        if (!hasStarted)
            return;
        
        _playerInput.SwitchCurrentActionMap(lastActionMap);
        pauseMenu.SetActive(false);
        cameraAnimator.SetTrigger("continue-camera");
        gradientAnimator.SetTrigger("fade-out");
        SetCursorLock(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
