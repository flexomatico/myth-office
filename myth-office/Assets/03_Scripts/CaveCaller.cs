using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCaller : MonoBehaviour
{
    public GameObject activeOffice;
    
    public void CallCave(GameObject nextOffice)
    {
        Destroy(activeOffice);
        activeOffice = Instantiate(nextOffice);
    }
}
