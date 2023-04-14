using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CaveLeaveHelper : MonoBehaviour
{
    public bool leaveOnPlayerEnterCAVE = false;
    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(DelayMoveAwayCave());
    }

    private IEnumerator DelayMoveAwayCave()
    {
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("SceneManager").GetComponent<SceneManager>().PlayerLeftCave();
    }

    private void OnTriggerEnter(Collider other)
    {
        StopCoroutine(DelayMoveAwayCave());
        if (leaveOnPlayerEnterCAVE) StartCoroutine(DelayGoToNextOffice());
    }

    private IEnumerator DelayGoToNextOffice()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject.Find("SceneManager").GetComponent<SceneManager>().GoToNextOffice();
    }
}
