
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneFinder))]
public class LevelPlacementController : MonoBehaviour
{

    [SerializeField] private LevelPositionManager _levelPositionManager;

    private CompatibilityChecker _compatibilityChecker;

    private ARSessionOrigin _arSessionOrigin;

    private ARPlaneFinder _arPlaneFinder;

    [SerializeField] private GameObject _levelPlaceholderPrefab;


    private GameObject _levelPlaceholder;

    private bool _hasPlacedLevel = false;

    public ARPlaneSearch LatestARPlaneSearch = new ARPlaneSearch() 
    {
        FoundPlane = false
    };

    void Awake()
    {
        _arSessionOrigin = this.gameObject.GetComponent<ARSessionOrigin>();
        this._arPlaneFinder = this.gameObject.GetComponent<ARPlaneFinder>();
    }

    void Start()
    {
        _levelPlaceholder = Instantiate(_levelPlaceholderPrefab, Vector3.zero, Quaternion.identity);
        _levelPlaceholder.SetActive(false);

        _compatibilityChecker = FindObjectOfType<CompatibilityChecker>();
        StartCoroutine(_compatibilityChecker.CheckForARSupport(this.PlaceLevelDesktop));
    }

    void Update()
    {
        if (!_hasPlacedLevel)
        {
            SearchForPlane();
            RenderLevelPlaceholderOnPlane();
            HandelUserTouch();
        }
    }

    private void HandelUserTouch()
    {
        bool hasStartedTouching = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (hasStartedTouching) PlaceLevel();
    }

    private void PlaceLevel()
    {
        if (!this.LatestARPlaneSearch.IsReasonableForLevelPlacement) return;
        
        _hasPlacedLevel = true;
        if (_levelPositionManager != null) {
            _levelPositionManager.UpdateLevelSpawnPosition(new Pose(
                this.LatestARPlaneSearch.PlaneHitPosition, 
                this.LatestARPlaneSearch.CameraRotationTowardsPlane
            ));
        }
        this.MoveToNextScene();
        
    }

    private void PlaceLevelDesktop(ARSessionState arSessionState)
    {
        if (arSessionState != ARSessionState.Unsupported) return;
        Debug.Log("Place level desktop");
        _hasPlacedLevel = true;
        this._levelPositionManager.UpdateLevelSpawnPosition(new Pose());
        this.MoveToNextScene();
    }

    private void SearchForPlane()
    {
        this.LatestARPlaneSearch = this._arPlaneFinder.SearchForPointedARPlane();
    }

    private void RenderLevelPlaceholderOnPlane()
    {
        if (!this.LatestARPlaneSearch.IsReasonableForLevelPlacement) 
        {
            _levelPlaceholder.SetActive(false);
        }
        else
        {
            _levelPlaceholder.SetActive(true);
            _levelPlaceholder.transform.SetPositionAndRotation(this.LatestARPlaneSearch.PlaneHitPosition, this.LatestARPlaneSearch.CameraRotationTowardsPlane);
        }
    }

    private void MoveToNextScene()
    {
        SceneManager.LoadScene("JoinGameSetup");
    }
}
