using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightPicker : MonoBehaviour
{
    private Camera _camera;

    private List<ColoredLight> _coloredLights;

    private float _pickRange = 4.0f;

    private ColoredLight _pickedLight;

    // Start is called before the first frame update
    void Start()
    {
        this._camera = GetComponent<Camera>();
        this._coloredLights = FindObjectsOfType<ColoredLight>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pickedLight == null)
        {
            CheckForLightsToPick();
        }
        else {
            MakeLightFollowCamera();
            CheckForDroppingLightInput();
        }
    }

    void CheckForLightsToPick()
    {
        var bestLight = GetBestPotentialLightToPick();
        foreach (var light in this._coloredLights)
        {
            light.ToggleLightPickHover(light == bestLight);
        }

        if (bestLight == null) return;
        if (ClickOrTap())
        {
            PickLight(bestLight);
        }
    }

    private void CheckForDroppingLightInput()
    {
        if (ClickOrTap())
        {
            Debug.Log("Drop Light");
            DropCurrentPickedLight();
        }
    }

    private bool ClickOrTap()
    {
        if (Touchscreen.current != null) return Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches.First().tap.wasPressedThisFrame;
        if (Mouse.current != null) return Mouse.current.leftButton.wasPressedThisFrame;
        return false;
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
            bool isFrontOfCamera = screenPoint.z > 0;
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
        Debug.Log(this._camera.transform.position);
        this._pickedLight.transform.position = this._camera.transform.position;
        this._pickedLight.transform.rotation = this._camera.transform.rotation;
    }

    private void PickLight(ColoredLight lightToSelect)
    {
        this._pickedLight = lightToSelect;
        this._pickedLight.ToggleLightPickSelect(true);
    }

    private void DropCurrentPickedLight()
    {
        this._pickedLight.ToggleLightPickSelect(false);
        this._pickedLight = null;
    }
}
