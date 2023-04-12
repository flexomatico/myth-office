using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCaller : MonoBehaviour
{
    public GameObject activeOffice;
    private GameObject nextOffice;
    public AudioClip arriveSound;
    public AnimationCurve animCurve;
    [HideInInspector] public Vector3 cavePosition;
    private AudioSource arriveSoundSource;

    public float travelDistance = 20.0f;
    public float travelTime = 10.0f;
    private float currentAnimTime = 0.0f;

    private void Start()
    {
        arriveSoundSource = gameObject.AddComponent<AudioSource>();
        arriveSoundSource.clip = arriveSound;
    }

    public void CallCave(bool playSound)
    {
        transform.position = Vector3.zero;
        if (playSound) arriveSoundSource.PlayDelayed(0.5f);
    }
    
    public void GoToNextOffice(GameObject _nextOffice)
    {
        nextOffice = Instantiate(_nextOffice);
        nextOffice.transform.position += new Vector3(0, travelDistance, 0);
        StartCoroutine(MoveOfficesVertically());
    }

    private IEnumerator MoveOfficesVertically()
    {
        while (currentAnimTime < 1.0f)
        {
            float nextYPos = animCurve.Evaluate(currentAnimTime) * travelDistance * -1;
            Vector3 activeOfficePos = new Vector3(0, nextYPos, 0);
            
            nextYPos += travelDistance;
            Vector3 nextOfficePos = new Vector3(0, nextYPos, 0);
            
            activeOffice.transform.position = activeOfficePos;
            nextOffice.transform.position = nextOfficePos;
            
            currentAnimTime += Time.deltaTime / travelTime;
            yield return null;
        }
        
        ArriveAtNextOffice();
    }

    private void ArriveAtNextOffice()
    {
        Destroy(activeOffice);
        activeOffice = nextOffice;
        activeOffice.transform.position = Vector3.zero;
        arriveSoundSource.Play();
    }
}
