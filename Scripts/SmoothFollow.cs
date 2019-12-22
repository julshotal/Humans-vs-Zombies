using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script smoothly follows around one gameobject 
//Code given to us in class
public class SmoothFollow : MonoBehaviour
{
    //set target and how far off the camera will follow
    public Transform target;
    public float distance = 5.0f;
    public float height = 1.50f;
    public float heightDamping = 2.0f;
    public float positionDamping = 2.0f;
    public float rotationDamping = 2.0f;

    //grab sceneManager script
    public GameObject manager;
    public AgentManager agentScript;

    // Start is called before the first frame update
    void Start()
    {
        //grab scene manager and it's script
        manager = GameObject.Find("Scene Manager");
        agentScript = manager.GetComponent<AgentManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //follow the original zombie
        target = agentScript.zombies[0].transform;

        // Early exit if there’s no target
        if (!target)  return;
        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;

        // Damp the height   
        currentHeight = Mathf.Lerp (currentHeight, wantedHeight,      heightDamping * Time.deltaTime);

        // Set the position of the camera    
        Vector3 wantedPosition = target.position - target.forward * distance;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);

        // Adjust the height of the camera   
        transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);

        // Set the forward to rotate with time   
        transform.forward = Vector3.Lerp (transform.forward, target.forward, Time.deltaTime * rotationDamping);
    }
}
