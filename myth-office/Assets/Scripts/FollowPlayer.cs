using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private GameObject _player;
    
    void Start()
    {
        _player = GameObject.Find("Player");
    }
    
    void Update()
    {
        transform.position = _player.transform.position + offset;
    }
}
