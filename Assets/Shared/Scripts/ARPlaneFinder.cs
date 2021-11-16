using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlaneFinder : MonoBehaviour
{
    private ARRaycastManager _arRaycastManager;
    [SerializeField] private Camera _arCamera;

    void Awake()
    {
        this._arRaycastManager = this.gameObject.GetComponent<ARRaycastManager>();
    }

    public ARPlaneSearch SearchForPointedARPlane()
    {
        Vector2 screenCenter = _arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        this._arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count == 0) return new ARPlaneSearch() { FoundPlane = false };
        
        Pose firstHitPose = hits[0].pose;
        Vector3 cameraForward = _arCamera.transform.forward;
        Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z);

        return new ARPlaneSearch() { 
            FoundPlane = true,
            PlaneHitPosition = firstHitPose.position,
            PlaneRotation = firstHitPose.rotation,
            CameraRotationTowardsPlane = Quaternion.LookRotation(cameraBearing),
            CameraTransform = _arCamera.transform,
        };
    }

}
public class ARPlaneSearch 
{
    public bool FoundPlane { set; get; } = false;
    public Vector3 PlaneHitPosition { set; get; } = Vector3.zero;
    public Quaternion PlaneRotation { set; get;}
    public Quaternion CameraRotationTowardsPlane { set; get; }
    public Transform CameraTransform { set; get; }

    public bool IsReasonableForLevelPlacement 
    {
        get {
            if (!FoundPlane) return false;

            float distanceToCamera = Vector3.Distance(CameraTransform.position, PlaneHitPosition);
            return (distanceToCamera > 2.0f && distanceToCamera < 50.0f);
        }
    }
}
