using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyHelper : MonoBehaviour
{
    public void CallCAVE()
    {
        GameObject.Find("SceneManager").GetComponent<SceneManager>().CallCave();
    }

    public void GoToNextOffice()
    {
        GameObject.Find("SceneManager").GetComponent<SceneManager>().GoToNextOffice();
    }
}
