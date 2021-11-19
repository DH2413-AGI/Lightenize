using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredLight : MonoBehaviour
{
    [SerializeField] private GameObject _uiContainer;

    /// <summary>If the light picker is close enough to pick this light</summary>
    private bool _lightPickerHover;

    private bool _lightPickerSelected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.TogglePickUI(this._lightPickerHover && !_lightPickerSelected);
    }

    public void ToggleLightPickHover(bool hover)
    {
        this._lightPickerHover = hover;

    }

    public void ToggleLightPickSelect(bool hover)
    {
        this._lightPickerSelected = hover;

    }

    private void TogglePickUI(bool enable)
    {   
        this._uiContainer.SetActive(enable);
    }
}
