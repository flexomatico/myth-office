using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<Journey> journeys;
    public GameObject activeOffice;
    public GameObject activeCave;
    private GameObject nextOffice;
    public AudioClip arriveSound;
    public AnimationCurve animCurve;
    private AudioSource arriveSoundSource;

    private float currentAnimTime = 0.0f;
    public int currentJourney = 0;
    private float travelDistance;
    private float travelTime;
    private bool currentJourneyInitiated = false;
    

    private bool caveCanLeave = true;

    private void Start()
    {
        arriveSoundSource = gameObject.AddComponent<AudioSource>();
        arriveSoundSource.clip = arriveSound;
        
        bool isStartOfGame = currentJourney == 0;
        if (isStartOfGame)
        {
            activeOffice = Instantiate(journeys[currentJourney].nextOffice);
            currentJourney++;
        }
        else
        {
            activeOffice = Instantiate(new GameObject("Empty GameObject"));
            activeCave = Instantiate(journeys[currentJourney].cave);
        }
    }

    public void CallCave()
    {
        currentJourneyInitiated = false;
        travelDistance = journeys[currentJourney].travelDistance;
        travelTime = journeys[currentJourney].travelTime;
        activeCave = Instantiate(journeys[currentJourney].cave);
        if (journeys[currentJourney].animateCaveArrival)
        {
            activeCave.transform.position += new Vector3(0, travelDistance, 0);
            currentAnimTime = 0.0f;
            StartCoroutine(MoveCaveVertically());
        }
        else
        {
            ArriveCave();
        }
    }
    
    public void GoToNextOffice()
    {
        if (currentJourneyInitiated) return;
        
        travelDistance = journeys[currentJourney].travelDistance;
        travelTime = journeys[currentJourney].travelTime;
        nextOffice = Instantiate(journeys[currentJourney].nextOffice);
        nextOffice.transform.position += new Vector3(0, travelDistance, 0);
        currentAnimTime = 0.0f;
        StartCoroutine(MoveOfficesVertically());
        currentJourneyInitiated = true;
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
        caveCanLeave = true;
        currentJourney++;
    }

    public void PlayerLeftCave()
    {
        if (caveCanLeave)
        {
            currentAnimTime = 0.0f;
            StartCoroutine(MoveCaveVertically());
        }
    }

    private IEnumerator MoveCaveVertically()
    {
        bool caveIsArriving = journeys[currentJourney].animateCaveArrival;
        float nextYPos;
        
        while (currentAnimTime < 1.0f)
        {
            if (caveIsArriving)
            {
                nextYPos = animCurve.Evaluate(1.0f - currentAnimTime) * travelDistance;
            }
            else
            {
                nextYPos = animCurve.Evaluate(currentAnimTime) * travelDistance;
            }
            Vector3 newPos = new Vector3(0, nextYPos, 0);
            activeCave.transform.position = newPos;
            
            currentAnimTime += Time.deltaTime / travelTime;
            yield return null;
        }
        
        if (caveIsArriving)
        {
            ArriveCave();
        }
        else
        {
            Destroy(activeCave);
        }
    }

    private void ArriveCave()
    {
        activeCave.transform.position = Vector3.zero;
        arriveSoundSource.PlayDelayed(0.5f);
        caveCanLeave = false;
    }
}
