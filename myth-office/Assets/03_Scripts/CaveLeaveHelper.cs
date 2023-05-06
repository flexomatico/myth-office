using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CaveLeaveHelper : MonoBehaviour
{
    public float waitForSeconds = 2.0f;
    private Coroutine delayMoveAwayCoroutine;
    private bool doorsAreClosed = false;
    
    private void OnTriggerExit(Collider other)
    {
        delayMoveAwayCoroutine = StartCoroutine(DelayMoveAwayCave());
    }

    private IEnumerator DelayMoveAwayCave()
    {
        if (doorsAreClosed)
        {
            yield break;
        }
        SceneManager sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        yield return new WaitForSeconds(0.5f);
        sceneManager.CloseElevatorDoors();
        doorsAreClosed = true;
        yield return new WaitForSeconds(1.0f);
        sceneManager.PlayerLeftCave();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (delayMoveAwayCoroutine != null && doorsAreClosed == false)
        {
            StopCoroutine(delayMoveAwayCoroutine);
        }
    }

    private IEnumerator DelayGoToNowhere()
    {
        yield return new WaitForSeconds(waitForSeconds);
        GameObject.Find("SceneManager").GetComponent<SceneManager>().GoToNowhere();
    }

    public void GoToNoWhere()
    {
        StartCoroutine(DelayGoToNowhere());
    }
}
