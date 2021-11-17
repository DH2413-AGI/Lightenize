using System;
using System.Collections;
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
        if (_arSession == null)
        {
            throw new ArgumentException("An ARSession needs to exist for the CompatibilityChecker");
        }

        StartCoroutine(CheckSupportForCurrentDevice());
    }

    public IEnumerator CheckSupportForCurrentDevice() {
        
        // AR
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }
        
        // VR
        // TODO
        
        if (ARSession.state == ARSessionState.Unsupported)
        {
            _supportedDevice = DeviceSupport.DESKTOP;
        }
        else
        {
            _supportedDevice = DeviceSupport.AR;
        }

        _listeners.Invoke(_supportedDevice);
    }

    public static void AddListener(Action<DeviceSupport> OnResult)
    {
        if (_supportedDevice != CompatibilityChecker.DeviceSupport.UNKNOWN)
        {
            OnResult.Invoke(_supportedDevice);
            return;
        }
        _listeners += OnResult;
    }
}

