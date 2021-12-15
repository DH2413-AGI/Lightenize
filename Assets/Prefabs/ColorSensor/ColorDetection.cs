using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorDetection : MonoBehaviour
{
    private enum TargetMode
    {
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

    private List<Color> _colorsOnSensor = new List<Color>();

    private List<Color> _colorsNeededForClear = new List<Color>();

    private Color _currentColor = new Color();

    private bool _hasMatchedTargetColor = false;

    /// <summary><summary>
    public bool IsCleared
    {
        get
        {
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

        if (IsCurrentColorMatchingGoalColor())
        {
            this._hasMatchedTargetColor = true;
            this.setClearColorCombination();
        }
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
        this._colorsOnSensor = new List<Color>();

        foreach (var coloredLight in this._coloredLights)
        {
            Vector3 lightPosition = coloredLight.transform.position;
            Vector3 directionToSensorFromLight = Vector3.Normalize(this.transform.position - lightPosition);
            var rayFromLightToSensor = new Ray(lightPosition, directionToSensorFromLight);

            // Angle between ray and forward vector of light
            float rayAngle = Vector3.Angle(coloredLight.transform.forward, -directionToSensorFromLight);

            if (!Physics.Raycast(rayFromLightToSensor, out hitInfo)) continue;
            if (rayAngle > coloredLight.OuterAngle / 2.0f || hitInfo.transform.gameObject != this.gameObject) continue;

            // Create a simple linear interpolation for the light fallout between the inner and outer angles.
            float linearInterpolationOuterToInnerAngle = Mathf.Clamp((coloredLight.OuterAngle - rayAngle * 2.0f) / (coloredLight.OuterAngle - coloredLight.InnerAngle), 0.0f, 1.0f);

            // calculates light falloff
            //float lightIntensity = coloredLight._spotLight.intensity;
            float distance = Vector3.Distance(lightPosition, this.transform.position);
            float lightRange = coloredLight.Range;
            //float attenuation = 1.0f / (distance * distance);
            float attenuation = 1.0f;

            // keep attenuation as 1 if the light is close
            if (distance > 5.0f)
                attenuation = Mathf.Pow(1.0f - Mathf.Clamp(distance / lightRange, 0.0f, 1.0f), 0.9f);
            //float attenuation = Mathf.Clamp(1.0f / (1.0f + (distance * distance)) * (1 - (distance * distance / lightRange)), 0.0f, 1.0f);

            Color attenuatedColor = new Color();
            attenuatedColor.r = Mathf.Min(coloredLight.Color.r * attenuation, 1.0f);
            attenuatedColor.g = Mathf.Min(coloredLight.Color.g * attenuation, 1.0f);
            attenuatedColor.b = Mathf.Min(coloredLight.Color.b * attenuation, 1.0f);

            this._currentColor += attenuatedColor * linearInterpolationOuterToInnerAngle;

            // Only use the interpolation to calculate what color is shone onto the sensor from the colored light
            Color colorFromColoredLight = coloredLight.Color * linearInterpolationOuterToInnerAngle;

            if (!this._colorsOnSensor.Contains(colorFromColoredLight)) this._colorsOnSensor.Add(colorFromColoredLight);
        }
    }

    private void UpdateUI()
    {
        _currentColorText.text = getColoredLightCombinationText();
        if (this.IsCleared)
        {
            _correctColorText.text = "Clear";
            _correctColorText.color = _currentColorText.color = ColorNames.GetColor("light green");
        }
        else
        {
            _correctColorText.text = "Make " + ColorNames.FindColor(this._targetColor);
            _currentColorText.color = ColorNames.GetColor("red");
        }
    }

    private float RoundToTwoDecimalPlaces(float valueToRound)
    {
        return Mathf.Round(valueToRound * 100.0f) / 100.0f;
    }

    public bool IsCurrentColorMatchingGoalColor()
    {
        return this._currentColor == this._targetColor;
    }

    private void setClearColorCombination()
    {

        if (this._colorsNeededForClear.Count == 0)
        {
            foreach (Color color in this._colorsOnSensor)
            {
                this._colorsNeededForClear.Add(color);
            }
        }
    }

    private string lightCombinationText(List<Color> colors, Color outcome)
    {
        if (ColorNames.FindColor(outcome) == "black") return "";

        if (colors.Count > 1)
        {
            string colorCombination = "";
            foreach (Color color in colors)
            {
                colorCombination += $" {ColorNames.FindColor(color)} +";
            }
            colorCombination = colorCombination.Remove(colorCombination.Length - 1);
            return colorCombination += $"\n= {ColorNames.FindColor(outcome)}";
        }
        else
        {
            return ColorNames.FindColor(outcome);
        }
    }

    private string getColoredLightCombinationText()
    {
        if (this.IsCleared)
        {
            return lightCombinationText(this._colorsNeededForClear, this._targetColor);
        }
        return lightCombinationText(this._colorsOnSensor, this._currentColor);
    }
}
