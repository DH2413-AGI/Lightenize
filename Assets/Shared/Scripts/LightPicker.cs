using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityCoreHaptics;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightPicker : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _pickUpSound;

    [SerializeField] private AudioClip _dropSound;

    private Camera _camera;

    private List<ColoredLight> _coloredLights;

    private float _pickRange = 4.0f;

    private ColoredLight _pickedLight;

    private ColoredLight _currentlyBestLightToPick;


    // Start is called before the first frame update
    void Start()
    {
        this._camera = GetComponent<Camera>();
        this._coloredLights = FindObjectsOfType<ColoredLight>().ToList();
        var playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Player").FindAction("PrimaryClick").performed += TogglePickDropLight;
    }

    // Update is called once per frame
    void Update()
    {
        if (_pickedLight != null)
        {
            MakeLightFollowCamera();
        }
        else
        {
            CheckForLightsToPick();
        }
    }

    void CheckForLightsToPick()
    {
        var bestLight = GetBestPotentialLightToPick();
        foreach (var light in this._coloredLights)
        {
            light.ToggleLightPickHover(light == bestLight);
        }

        this._currentlyBestLightToPick = bestLight;
    }

    private ColoredLight GetBestPotentialLightToPick()
    {
        var lightsWithinRange = FilterOutColoredLightOutsideRange(this._coloredLights);

        if (lightsWithinRange.Count == 0) return null;
        var lightWithinRangeAndViewport = FilterOutColoredLightsOutsideViewport(lightsWithinRange);

        if (lightWithinRangeAndViewport.Count == 0) return null;
        var closestLight = GetClosestColoredLight(lightWithinRangeAndViewport);
        return closestLight;
    }

    private ColoredLight GetClosestColoredLight(List<ColoredLight> coloredLights)
    {
        return coloredLights.Aggregate((light1, light2) => this.DistanceToLight(light1) < this.DistanceToLight(light2) ? light1 : light2);
    }

    private List<ColoredLight> FilterOutColoredLightsOutsideViewport(List<ColoredLight> coloredLights)
    {
        return coloredLights.FindAll((coloredLight) => {
            Vector3 screenPoint = _camera.WorldToViewportPoint(coloredLight.transform.position);
            bool isFrontOfCamera = screenPoint.z > -0.1f;
            bool isInsideVerticalAxis = screenPoint.y > 0 & screenPoint.y < 1;
            bool isInsideHorizontalAxis = screenPoint.x > 0 & screenPoint.x < 1;
            bool isInsideViewport = isFrontOfCamera && isInsideVerticalAxis && isInsideHorizontalAxis;
            return isInsideViewport;
        }
        ).ToList();
    }

    private List<ColoredLight> FilterOutColoredLightOutsideRange(List<ColoredLight> coloredLights)
    {
        return coloredLights.FindAll(coloredLight => 
            this.DistanceToLight(coloredLight) <= _pickRange
        ).ToList();
    }

    private float DistanceToLight(ColoredLight light)
    {
        return Vector3.Distance(light.transform.position, this.transform.position);
    }

    private void MakeLightFollowCamera()
    {
        var newTargetPosition = this._camera.transform.position + this._camera.transform.forward * 1.5f + this._camera.transform.up * -0.4f;
        this._pickedLight.transform.position = Vector3.Lerp(this._pickedLight.transform.position, newTargetPosition, Time.deltaTime * 10.0f);
        this._pickedLight.transform.rotation = this._camera.transform.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

    private void TogglePickDropLight(InputAction.CallbackContext context)
    {
        if (this._pickedLight == null && this._currentlyBestLightToPick != null)
        {
            PickLight();
        }
        else 
        {
            DropCurrentPickedLight();
        }
    }

    private void PickLight()
    {
        if (this._currentlyBestLightToPick == null) return;
        this._pickedLight = this._currentlyBestLightToPick;
        this._pickedLight.ToggleLightPickSelect(true);
        this._audioSource.PlayOneShot(this._pickUpSound);
        if (UnityCoreHapticsProxy.SupportsCoreHaptics())
        {
            UnityCoreHapticsProxy.PlayTransientHaptics(1.0f, 0.0f);
        }
    }

    private void DropCurrentPickedLight()
    {
        if (this._pickedLight == null) return;
        this._pickedLight.ToggleLightPickSelect(false);
        this._pickedLight.ToggleLightPickHover(false);
        this._pickedLight = null;
        this._audioSource.PlayOneShot(this._dropSound);
        if (UnityCoreHapticsProxy.SupportsCoreHaptics())
        {
            UnityCoreHapticsProxy.PlayTransientHaptics(1.0f, 1.0f);
        }
    }
}
