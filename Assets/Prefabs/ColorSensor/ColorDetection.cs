using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorDetection : MonoBehaviour
{
    [SerializeField] private Color _goalColor;

    [SerializeField] private GameObject _goalColorObject;

    [SerializeField] private TMP_Text _currentColorText;
    [SerializeField] private TMP_Text _correctColorText;
    
    private Material _goalColorMaterial;

    private ColoredLight[] _coloredLights;

    private Color _currentColor = new Color();

    void Start()
    {
        this._coloredLights = FindObjectsOfType<ColoredLight>();
        this._goalColorMaterial = this._goalColorObject.GetComponent<Renderer>().material;
        this.UpdateGoalColorMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        this.CheckForCurrentColorOnSensor();
        this.UpdateGoalColorMaterial();
        this.UpdateUI();
    }

    private void UpdateGoalColorMaterial()
    {
        this._goalColorMaterial.SetColor("_Color", this._goalColor);
        this._goalColorMaterial.SetColor("_EmissionColor", this._goalColor * 3.0f);
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
        if (this._currentColor == this._goalColor)
        {
            _correctColorText.text = "Correct";
            _correctColorText.color = Color.green;
        }
        else 
        {
            _correctColorText.text = "Wrong";
            _correctColorText.color = Color.red;
        }
        _currentColorText.text = $"Red: {RoundToTwoDecimalPlaces(this._currentColor.r)}, Green: {RoundToTwoDecimalPlaces(this._currentColor.g)}, Blue: {RoundToTwoDecimalPlaces(this._currentColor.b)}";
    }
    
    private float RoundToTwoDecimalPlaces(float valueToRound)
    {
        return Mathf.Round(valueToRound * 100.0f) / 100.0f;
    }
}
