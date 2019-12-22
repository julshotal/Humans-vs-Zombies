using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script to run circle collision
public class Collisions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //method to run collision between humans and zombies
    public bool CircleCollision(GameObject human, GameObject zombie)
    {
        //calculate the distance between center points using the pythagorean theorm
        float distance = Mathf.Pow((human.transform.position.x - zombie.transform.position.x), 2) + Mathf.Pow((human.transform.position.z - zombie.transform.position.z), 2);
        distance = Mathf.Sqrt(distance);

        //if the distance between center points is less than the addition of both radi, return true
        if (distance < 1.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
