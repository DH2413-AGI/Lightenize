using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ColoredLight : MonoBehaviour
{
    [SerializeField] private PickLightUI _pickUI;

    [SerializeField] private Color _color;

    [SerializeField] private Light _spotLight;

    [SerializeField] private Light _pointLight;

    [SerializeField] private GameObject _colorIndicator;

    [SerializeField] private GameObject _lightCone;
    [SerializeField] private AudioSource _audioSource;

    /// <summary> If the light picker is close enough to pick this light </summary>
    private bool _lightPickerHover;

    private bool _lightPickerSelected;

    private Vector3 _lastPosition = Vector3.zero;
    public Color Color
    {
        get => this._color;
    }

    public float InnerAngle
    {
        get => this._spotLight.innerSpotAngle;
    }

    public float OuterAngle
    {
        get => this._spotLight.spotAngle;
    }
    public float Range
    {
        get => this._spotLight.range;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            SetColorToShaderGraph();
            SetColorToMaterials();
        }  
        this._lastPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            this.TogglePickUI(this._lightPickerHover, _lightPickerSelected);
        }
        SetColorToLights();
        HandleSounds();
        this._lastPosition = this.transform.position;
    }

    private void HandleSounds()
    {
        float amountOfMovement = Vector3.Distance(this._lastPosition, this.transform.position);
        if (amountOfMovement < 0.001f) return;
        this._audioSource.volume = Mathf.Clamp(amountOfMovement * 7.5f, 0.0f, 1.0f);
    }

    private void SetColorToMaterials()
    {
        var emission = 3.3f;
        var material = this._colorIndicator.GetComponent<Renderer>().material;
        material.SetColor("_Color", this._color);
        material.SetColor("_EmissionColor", this._color * emission);
    }

    private void SetColorToLights()
    {
        this._spotLight.color = this._color;
        this._pointLight.color = this._color;
    }

    public void ToggleLightPickHover(bool hover)
    {
        this._lightPickerHover = hover;

    }

    public void ToggleLightPickSelect(bool hover)
    {
        this._lightPickerSelected = hover;

    }

    private void TogglePickUI(bool enable, bool isGrabbing)
    {
        this._pickUI.toggleGrabbing(enable, isGrabbing);
    }

    private void SetColorToShaderGraph()
    {
        var lightConeMaterial = this._lightCone.GetComponent<Renderer>().material;
        lightConeMaterial.SetVector("_color", this._color);
    }
}