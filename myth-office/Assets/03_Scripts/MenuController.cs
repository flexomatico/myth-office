using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        eventSystem.SetSelectedGameObject(playButton);
        _playerInput.SwitchCurrentActionMap("UI");
    }

    public void StartGame()
    {
        _playerInput.SwitchCurrentActionMap("Player");
        startMenu.SetActive(false);
        cameraAnimator.SetTrigger("continue-camera");
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        _playerInput.SwitchCurrentActionMap("UI");
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(resumeButton);
        cameraAnimator.SetTrigger("pause-camera");
    }

    public void ContinueGame()
    {
        _playerInput.SwitchCurrentActionMap("Player");
        pauseMenu.SetActive(false);
        cameraAnimator.SetTrigger("continue-camera");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
