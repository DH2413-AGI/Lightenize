using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorDetection : MonoBehaviour
{
    private enum TargetMode {
        Once,
        Always
    }

    [Header("Individual Settings")]
    [SerializeField] private Color _targetColor;
    [SerializeField] private TargetMode _targetMode = TargetMode.Once;

    [Header("Prefab Settings")]
    [SerializeField] private GameObject _targetColorObject;
    [SerializeField] private TMP_Text _currentColorText;
    [SerializeField] private TMP_Text _correctColorText;
    
    private Material _goalColorMaterial;

    private ColoredLight[] _coloredLights;

    private Color _currentColor = new Color();

    private bool _hasMatchedTargetColor = false;

    /// <summary><summary>
    public bool IsCleared 
    {
        get {
            if (this._targetMode == TargetMode.Once) return this._hasMatchedTargetColor;
            else return this.IsCurrentColorMatchingGoalColor();
        }
    }

    void Start()
    {
        this._coloredLights = FindObjectsOfType<ColoredLight>();
        this._goalColorMaterial = this._targetColorObject.GetComponent<Renderer>().material;
        this.UpdateGoalColorMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        this.CheckForCurrentColorOnSensor();
        this.UpdateGoalColorMaterial();
        this.UpdateUI();

        if (IsCurrentColorMatchingGoalColor()) this._hasMatchedTargetColor = true;
    }

    private void UpdateGoalColorMaterial()
    {
        this._goalColorMaterial.SetColor("_Color", this._targetColor);
        this._goalColorMaterial.SetColor("_EmissionColor", this._targetColor * 3.0f);
    }

    private void CheckForCurrentColorOnSensor()
    {
        RaycastHit hitInfo;

        this._currentColor = new Color();

        foreach (var coloredLight in this._coloredLights)
        {
            Vector3 lightPosition = coloredLight.transform.position;
            Vector3 directionToSensorFromLight = Vector3.Normalize(this.transform.position - lightPosition);
            var rayFromLightToSensor = new Ray(lightPosition, directionToSensorFromLight);

            // Angle between ray and forward vector of light
            float rayAngle = Vector3.Angle(coloredLight.transform.forward, -directionToSensorFromLight);

            if (!Physics.Raycast(rayFromLightToSensor, out hitInfo)) continue;
            if (rayAngle > coloredLight.OuterAngle / 2.0f || hitInfo.transform.gameObject != this.gameObject) continue;

            // Create a simpel linear interpolation for the light fallout between the inner and outer angles.
            float linearInterpolationOuterToInnerAngle = Mathf.Clamp((coloredLight.OuterAngle - rayAngle * 2.0f) / (coloredLight.OuterAngle - coloredLight.InnerAngle), 0.0f, 1.0f);
            this._currentColor += coloredLight.Color * linearInterpolationOuterToInnerAngle;
        }

    }

    private void UpdateUI()
    {
        if (this.IsCleared)
        {
            _correctColorText.text = "Correct";
            _correctColorText.color = Color.green;
        }
        else 
        {
            _correctColorText.text = "Wrong";
            _correctColorText.color = Color.red;
        }
        _currentColorText.text = 
            $"Red: {RoundToTwoDecimalPlaces(this._currentColor.r)}, " +  
            $"Green: {RoundToTwoDecimalPlaces(this._currentColor.g)}, " +  
            $"Blue: {RoundToTwoDecimalPlaces(this._currentColor.b)}, ";
    }
    
    private float RoundToTwoDecimalPlaces(float valueToRound)
    {
        return Mathf.Round(valueToRound * 100.0f) / 100.0f;
    }

    public bool IsCurrentColorMatchingGoalColor()
    {
        return this._currentColor == this._targetColor;
    }
}
