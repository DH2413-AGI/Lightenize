using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPlatformCamera : MonoBehaviour
{
    [SerializeField] private GameObject _arContainer;
    [SerializeField] private GameObject _vrContainer;
    [SerializeField] private GameObject _desktopContainer;
    
    // Start is called before the first frame update
    void Start()
    {
        CompatibilityChecker.AddListener(EnableCorrectCamera);
    }

    private void EnableCorrectCamera(CompatibilityChecker.DeviceSupport deviceSupport)
    {
        switch (deviceSupport)
        {
            case CompatibilityChecker.DeviceSupport.AR:
                EnableARCamera();
                return;
            default:
                EnableDesktopCamera();
                return;
        }
    }

    public void EnableARCamera()
    {
        this._vrContainer.SetActive(false);
        this._desktopContainer.SetActive(false);
        this._arContainer.SetActive(true);
    }

    public void EnableDesktopCamera()
    {
        this._vrContainer.SetActive(false);
        this._arContainer.SetActive(false); 
        this._desktopContainer.SetActive(true);
    }

}
