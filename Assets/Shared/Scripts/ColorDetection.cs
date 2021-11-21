using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDetection : MonoBehaviour
{

    public float red, blue, green;

    private GameObject[] spotLights;        // all the spot lights in the scene.

    // Start is called before the first frame update
    void Start()
    {
        spotLights = GameObject.FindGameObjectsWithTag("SpotLight");        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        float redEachFrame = 0, greenEachFrame = 0, blueEachFrame = 0;

        // get angle, color
        for (int i=0; i < spotLights.Length; i++)
        {
            Light spotlight = spotLights[i].GetComponent<Light>();
            spotlight.type = LightType.Spot;
            Color lightColor = spotlight.color;
            float spotAngle = spotlight.innerSpotAngle / 2.0f;
            Vector3 forward = spotlight.transform.forward;
            //Debug.Log("light " + i + " color: " + lightColor);
            // Debug.Log("angle: " + angle);
            //Debug.Log("range: " + spotLights[i].GetComponent<Light>().range);
            
            Vector3 localPos = transform.position;
            Vector3 lightPos = spotLights[i].transform.position;
            Vector3 lightToSensor = Vector3.Normalize(localPos - lightPos);

            var ray = new Ray(lightPos, lightToSensor);
            float rayAngle = Vector3.Angle(forward, lightToSensor);     // angle between ray and forward vector of light
            float angleInBetween = spotAngle - rayAngle;
            // Debug.Log("angle in between: " + angleInBetween); 

            if (Physics.Raycast(ray, out hitInfo))
            {
                // Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.blue);
                if (rayAngle <= spotAngle && hitInfo.transform.gameObject.tag == "ColorSensor")
                {
                    redEachFrame += lightColor.r;
                    greenEachFrame += lightColor.g;
                    blueEachFrame += lightColor.b;
                }
            }
        }

        red = (int)(Mathf.Min(redEachFrame * 255, 255));
        green = (int)(Mathf.Min(greenEachFrame * 255, 255));
        blue = (int)(Mathf.Min(blueEachFrame * 255, 255)); 

        Debug.Log("Red: "+ red +" Green: "+ green + " blue: "+ blue);
    }
}
