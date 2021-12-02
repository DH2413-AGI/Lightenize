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

    private List<ColoredLight> _coloredLightsPointingAtSensor = new List<ColoredLight>();

    private List<ColoredLight> _coloredLightsNeededForClear = new List<ColoredLight>();

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
            this.setClearColoredLightsCombination();
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
        this._coloredLightsPointingAtSensor = new List<ColoredLight>();

        foreach (var coloredLight in this._coloredLights)
        {
            Vector3 lightPosition = coloredLight.transform.position;
            Vector3 directionToSensorFromLight = Vector3.Normalize(this.transform.position - lightPosition);
            var rayFromLightToSensor = new Ray(lightPosition, directionToSensorFromLight);

            // Angle between ray and forward vector of light
            float rayAngle = Vector3.Angle(coloredLight.transform.forward, -directionToSensorFromLight);

            if (!Physics.Raycast(rayFromLightToSensor, out hitInfo)) continue;
            if (rayAngle > coloredLight.OuterAngle / 2.0f || hitInfo.transform.gameObject != this.gameObject) continue;

            if (!this._coloredLightsPointingAtSensor.Contains(coloredLight)) this._coloredLightsPointingAtSensor.Add(coloredLight);

            // Create a simpel linear interpolation for the light fallout between the inner and outer angles.
            float linearInterpolationOuterToInnerAngle = Mathf.Clamp((coloredLight.OuterAngle - rayAngle * 2.0f) / (coloredLight.OuterAngle - coloredLight.InnerAngle), 0.0f, 1.0f);
            this._currentColor += coloredLight.Color * linearInterpolationOuterToInnerAngle;
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

    private void setClearColoredLightsCombination()
    {

        if (this._coloredLightsNeededForClear.Count == 0)
        {
            foreach (ColoredLight cl in this._coloredLightsPointingAtSensor)
            {
                this._coloredLightsNeededForClear.Add(cl);
            }
        }
    }

    private string lightCombinationText(List<ColoredLight> lights, Color outcome)
    {
        if (ColorNames.FindColor(outcome) == "black") return "";

        if (lights.Count > 1)
        {
            string colorCombination = "";
            foreach (ColoredLight cl in lights)
            {
                colorCombination += $" {ColorNames.FindColor(cl.Color)} +";
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
            return lightCombinationText(this._coloredLightsNeededForClear, this._targetColor);
        }
        return lightCombinationText(this._coloredLightsPointingAtSensor, this._currentColor);
    }
}
