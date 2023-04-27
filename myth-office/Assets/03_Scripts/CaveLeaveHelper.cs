using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CaveLeaveHelper : MonoBehaviour
{
    public bool leaveOnPlayerEnterCAVE = false;
    public float waitForSeconds = 2.0f;
    private Coroutine delayMoveAwayCoroutine;
    private void OnTriggerExit(Collider other)
    {
        delayMoveAwayCoroutine = StartCoroutine(DelayMoveAwayCave());
    }

    private IEnumerator DelayMoveAwayCave()
    {
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("SceneManager").GetComponent<SceneManager>().PlayerLeftCave();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (delayMoveAwayCoroutine != null)
        {
            StopCoroutine(delayMoveAwayCoroutine);
        }
        if (leaveOnPlayerEnterCAVE) StartCoroutine(DelayGoToNowhere());
    }

    private IEnumerator DelayGoToNowhere()
    {
        yield return new WaitForSeconds(waitForSeconds);
        GameObject.Find("SceneManager").GetComponent<SceneManager>().GoToNowhere();
    }
}
