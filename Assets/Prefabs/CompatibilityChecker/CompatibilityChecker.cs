using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CompatibilityChecker : MonoBehaviour
{
    public enum DeviceSupport
    {
        AR,
        VR,
        DESKTOP,
        UNKNOWN,
    }

    private ARSession _arSession;
    private static DeviceSupport _supportedDevice = DeviceSupport.UNKNOWN;
    private static Action<DeviceSupport> _listeners;

    void Start()
    {
        _arSession = FindObjectOfType<ARSession>();
        if (_arSession)
        {
            throw new ArgumentException("An ARSession needs to exist for the CompatibilityChecker");
        }
    }

    public IEnumerator CheckForARSupport(Action<ARSessionState> OnCheckCompleted) {
        
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }
        OnCheckCompleted(ARSession.state);
    }

    public static void AddListener(Action<DeviceSupport> OnResult)
    {
        if (_supportedDevice == CompatibilityChecker.DeviceSupport.UNKNOWN)
            _listeners += OnResult;
    }
}

