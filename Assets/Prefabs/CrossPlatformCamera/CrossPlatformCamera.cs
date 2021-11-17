using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CrossPlatformCamera : MonoBehaviour
{
    [SerializeField] private GameObject _arContainer;
    [SerializeField] private GameObject _vrContainer;
    [SerializeField] private GameObject _desktopContainer;

    [Header("Augmented Reality")]
    [SerializeField] private ARSessionOrigin _arSessionOrigin;

    void Start()
    {
        CompatibilityChecker.AddListener(EnableCorrectCamera);
    }

    void Update()
    {

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

    private void MakeOriginAppearAt(Pose levelPlacementPose)
    {
        _arSessionOrigin.MakeContentAppearAt(new GameObject().transform, levelPlacementPose.position, levelPlacementPose.rotation);
    }

}
