using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script allows users to cycle through different cameras by pressing "C"
public class ChangeView : MonoBehaviour
{
    //Camera array that holds a reference to every camera in the scene
    public Camera[] cameras;

    // Current camera 
    private int currentCameraIndex;
    public string currentCameraName;

    // Use this for initialization
    void Start()
    {
        //grab all active camras within the scene
        cameras = Camera.allCameras;

        //start on first camera
        currentCameraIndex = 0;
        currentCameraName = cameras[currentCameraIndex].name;

        // Turn all cameras off, except the first default one
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // If any cameras were added to the controller, enable the first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Press the 'C' key to cycle through cameras in the array
        if (Input.GetKeyDown(KeyCode.C))
        {

            // Cycle to the next camera
            currentCameraIndex++;

            // If cameraIndex is in bounds, set this camera active and last one inactive
            if (currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
                currentCameraName = cameras[currentCameraIndex].name;
            } //If last camera, cycle back to first camera
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0; cameras[currentCameraIndex].gameObject.SetActive(true);
                currentCameraName = cameras[currentCameraIndex].name;
            }
        }
    }

    //GUI telling users what camera theyre on
    void OnGUI()
    {
        GUI.color = Color.cyan;
        GUI.skin.box.fontSize = 20;
        GUI.Box(new Rect(10, 100, 300, 35), currentCameraName);
        GUI.skin.box.wordWrap = true;
    }

}
