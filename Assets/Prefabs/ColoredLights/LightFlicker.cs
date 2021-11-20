using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{

    [SerializeField] float _perlineNoiseOffset = 0.0f;
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _intensity = 0.2f;

    private float _perlineNoiseCoord = 0.0f;

    private Light _light;

    private float _originalIntensity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        this._light = GetComponent<Light>();
        _originalIntensity = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        this._perlineNoiseCoord += _speed * Time.deltaTime;
        this._light.intensity = _originalIntensity * ((1 - Mathf.PerlinNoise(this._perlineNoiseCoord, _perlineNoiseOffset) * _intensity));
    }
}
