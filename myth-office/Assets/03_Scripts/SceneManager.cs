using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
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
    public float defaultTravelDistance = 30.0f;
    private float travelTime;
    private float travelDistance;
    private bool currentJourneyInitiated = false;
    public float caveAutoArriveSpeed = 5;
    public float caveAutoLeaveSpeed = 2;
    

    private bool caveCanLeave = true;
    private Coroutine animationCoroutine = null;

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
        activeCave = Instantiate(journeys[currentJourney].cave);
        if (journeys[currentJourney].animateCaveArrival)
        {
            travelTime = caveAutoArriveSpeed;
            travelDistance = defaultTravelDistance;
            activeCave.transform.position += new Vector3(0, travelDistance, 0);
            currentAnimTime = 0.0f;
            StartCoroutine(AnimateCaveArrival());
        }
        else
        {
            ArriveCave();
        }
    }
    
    public void GoToNextOffice()
    {
        if (currentJourneyInitiated) return;
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            ArriveAtNextOffice(false);
            animationCoroutine = null;
        }
        travelTime = journeys[currentJourney].travelTime;
        travelDistance = journeys[currentJourney].travelDistance;
        nextOffice = Instantiate(journeys[currentJourney].nextOffice);
        nextOffice.transform.position += new Vector3(0, travelDistance, 0);
        currentAnimTime = 0.0f;
        StartCoroutine(MoveOfficesVertically(true));
        currentJourneyInitiated = true;
    }

    public void GoToNowhere()
    {
        if (currentJourneyInitiated) return;
        
        travelTime = caveAutoLeaveSpeed;
        travelDistance = defaultTravelDistance;
        nextOffice = Instantiate(new GameObject("Empty GameObject"));
        nextOffice.transform.position += new Vector3(0, travelDistance, 0);
        currentAnimTime = 0.0f;
        animationCoroutine = StartCoroutine(MoveOfficesVertically(false));
    }

    private IEnumerator MoveOfficesVertically(bool arriveWithSound)
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
        
        ArriveAtNextOffice(arriveWithSound);
    }

    private void ArriveAtNextOffice(bool arriveWithSound)
    {
        Destroy(activeOffice);
        activeOffice = nextOffice;
        activeOffice.transform.position = Vector3.zero;
        caveCanLeave = true;
        if (arriveWithSound)
        {
            arriveSoundSource.Play();
        }
        else
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    public void PlayerLeftCave()
    {
        if (caveCanLeave)
        {
            currentAnimTime = 0.0f;
            StartCoroutine(AnimateCaveDeparture());
        }
    }

    private IEnumerator AnimateCaveDeparture()
    {
        bool caveIsArriving = journeys[currentJourney].animateCaveArrival;
        float nextYPos;
        
        while (currentAnimTime < 1.0f)
        {
            nextYPos = animCurve.Evaluate(currentAnimTime) * travelDistance;
            
            Vector3 newPos = new Vector3(0, nextYPos, 0);
            activeCave.transform.position = newPos;
            
            currentAnimTime += Time.deltaTime / travelTime;
            yield return null;
        }
        
        Destroy(activeCave);
        currentJourney++;
    }

    private IEnumerator AnimateCaveArrival()
    {
        bool caveIsArriving = journeys[currentJourney].animateCaveArrival;
        float nextYPos;
        
        while (currentAnimTime < 1.0f)
        {
            nextYPos = animCurve.Evaluate(1.0f - currentAnimTime) * travelDistance;
            
            Vector3 newPos = new Vector3(0, nextYPos, 0);
            activeCave.transform.position = newPos;
            
            currentAnimTime += Time.deltaTime / travelTime;
            yield return null;
        }
        
        ArriveCave();
    }

    private void ArriveCave()
    {
        activeCave.transform.position = Vector3.zero;
        arriveSoundSource.PlayDelayed(0.5f);
        caveCanLeave = false;
    }
}
