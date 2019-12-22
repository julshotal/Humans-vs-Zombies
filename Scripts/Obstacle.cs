using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for all obstacle objects to hold their positiona and radius
public class Obstacle : MonoBehaviour
{
    //set radius and grab obstacle's position
    public float radius;
    public Vector3 position;

    // Start is called before the first frame update
    void Start()
    { 
        //set position to current transform position
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
