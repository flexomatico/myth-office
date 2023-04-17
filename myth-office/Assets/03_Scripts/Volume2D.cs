using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class Volume2D : MonoBehaviour
{
    private Transform listenerTransform;
    private AudioSource audioSource;
    private float maxVolume;
    public float minDist=1;
    public float maxDist=20;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxDist);
        Gizmos.DrawWireSphere(transform.position, minDist);
    }

    public void Start()
    {
        listenerTransform = GameObject.Find("Player").transform;
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, listenerTransform.position);
 
        if(dist < minDist)
        {
            audioSource.volume = maxVolume;
        }
        else if(dist > maxDist)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = Remap(1.0f - ((dist - minDist) / (maxDist - minDist)), 0.0f, 1.0f, 0.0f, maxVolume);
            print(audioSource.volume);
        }
    }
    
    private float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}