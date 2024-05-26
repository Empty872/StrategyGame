using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private CinemachineImpulseSource _cinemachineImpulseSource;
    public static ScreenShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float force = 1f)
    {
        _cinemachineImpulseSource.GenerateImpulse(force);
    }
}
