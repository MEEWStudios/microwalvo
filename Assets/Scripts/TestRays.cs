//Attach this script to your Camera
//This draws a line in the Scene view going through a point 200 pixels from the lower-left corner of the screen
//To see this, enter Play Mode and switch to the Scene tab. Zoom into your Camera's position.
using UnityEngine;
using System.Collections;

public class TestRays : MonoBehaviour
{
    Camera cam;
    public GameObject Spotlight;
    public GameObject TopViewCamera;


    void Start()
    {
        //cam = GetComponent<Camera>();
    }

    void Update()
    {
        //Ray ray = cam.ScreenPointToRay(Spotlight.transform.position);
        Debug.DrawRay(Spotlight.transform.position, (TopViewCamera.transform.position - Spotlight.transform.position), Color.yellow);
    }
}