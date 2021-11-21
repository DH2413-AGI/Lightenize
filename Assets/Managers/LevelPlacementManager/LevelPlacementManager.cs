using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class LevelPlacementManager : MonoBehaviour
{
    [SerializeField] GameObject _levelPlaceholder;
    
    private ARRaycastManager _arRaycastManager;

    private List<RaycastHit> _raycastHits;

    private LevelPlacementData _levelPlacementData;

    private Camera _arCamera;

    private Pose _latestPlaneHit = new Pose(Vector3.zero, Quaternion.identity);

    private LevelManager _levelManager;

    private InputAction _primaryClickAction;

    // Start is called before the first frame update
    void Start()
    {
        CompatibilityChecker.AddListener(delegate (CompatibilityChecker.DeviceSupport supportedDevice)  {
            if (supportedDevice != CompatibilityChecker.DeviceSupport.AR) {
                Destroy(this.gameObject);
                return;
            }

            this._arRaycastManager = FindObjectOfType<ARRaycastManager>();
            this._arCamera = FindObjectOfType<Camera>();
            this._levelPlacementData = FindObjectOfType<LevelPlacementData>();
            this._levelManager = FindObjectOfType<LevelManager>();

            var playerInput = FindObjectOfType<PlayerInput>();
            _primaryClickAction = playerInput.actions.FindActionMap("Player").FindAction("PrimaryClick");
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!SetupManager.IsSetupCompleted) return;
        this.LookForPlane();
        this.RenderLevelPlacementPlaceholder();

        if (_primaryClickAction.WasPerformedThisFrame())
        {
            PlaceLevel();
        }
    }

    private void LookForPlane()
    {
        Vector2 screenCenter = _arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        this._arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        if (hits.Count == 0) return;
        Vector3 cameraForward = _arCamera.transform.forward;
        Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z);
        Quaternion hitRotation = Quaternion.LookRotation(cameraBearing);

        this._latestPlaneHit = new Pose(hits[0].pose.position, hitRotation);
    }

    private void RenderLevelPlacementPlaceholder()
    {
        if (this._latestPlaneHit == null) return;
        _levelPlaceholder.transform.SetPositionAndRotation(this._latestPlaneHit.position, this._latestPlaneHit.rotation);
    }

    private void PlaceLevel()
    {
        if (this._latestPlaneHit == null) return;
        ChoosePositionAndRotationOfLevels(_latestPlaneHit);
    }

    private void ChoosePositionAndRotationOfLevels(Pose pose)
    {
        _levelPlacementData.SetLevelPose(pose);
        MoveToNextScene();
    }
    
    private void MoveToNextScene()
    {
        this._levelManager.LoadNextLevel();
    }
}
