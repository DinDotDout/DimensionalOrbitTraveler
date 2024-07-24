using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Menu : MonoBehaviour
{
    public Transform planet;

    float mouseSensivity = 30f;

    float smoothedPitch;
    float smoothedYaw;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        smoothedYaw = mouseX;
        if (x >= Screen.width || x <= 0)
        {
            smoothedYaw = 0f;
        }
        else
        {
            float damp;
            if (x >= Screen.width/2)
            {
                damp = (x - Screen.width/2) / (Screen.width / 2);
                smoothedYaw *= 1 - Mathf.Sin(damp * Mathf.PI * 0.5f);
            }
            if (x < Screen.width/2)
            {
                damp = (Screen.width / 2 - x) / (Screen.width / 2);
                smoothedYaw *= 1 - Mathf.Sin(damp * Mathf.PI * 0.5f);
            }
        }

        smoothedPitch = mouseY;
        if (y >= Screen.height || y <= 0)
        {
            smoothedPitch = 0f;
        }
        else
        {
            float damp;
            if (y >= Screen.height / 2)
            {
                damp = (y - Screen.height / 2) / (Screen.height / 2);
                smoothedPitch *= 1 - Mathf.Sin(damp * Mathf.PI * 0.5f);
            }
            if (y < Screen.height / 2)
            {
                damp = (Screen.height / 2 - y) / (Screen.height / 2);
                smoothedPitch *= 1 - Mathf.Sin(damp * Mathf.PI * 0.5f);
            }
        }

        transform.RotateAround(planet.position, transform.right, smoothedPitch);
        transform.RotateAround(planet.position, transform.up, smoothedYaw);
    }
}
