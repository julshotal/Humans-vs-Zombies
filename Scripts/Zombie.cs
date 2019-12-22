using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The zombie class with pursue the nearest human until all humans are bitten, and then will wander around the terrain
public class Zombie : Vehicle
{
    //variable for nearest human
    GameObject closestHuman;
    public Material futureMat;

    //material for human tracking debug line
    public Material humanMat;

    // Start is called before the first frame update
    public new void Start()
    {
        //grab Vehicle's start()
        base.Start();

        //the y of the zombie object will always be 1.5
        defaultY = 1.5f;
    }

    // Update is called once per frame
    public new void Update()
    {
        //grab Vehicle's update()
        base.Update();

        //run CheckCollision from the scenemanager
        agentScript.CheckCollision();

        //avoid all obstacles
        foreach (GameObject obstacle in agentScript.obstacles)
        {
            ApplyForce(ObstacleAvoidance(15, 20, obstacle) * 8f);
        }

    }

    public override void CalcSteeringForces()
    {
        //run the method from the scene manager to remove all dead humans
        agentScript.RemoveHumans();

        //set ultimate force to 0 and the closestDistance to a huge numbers
        Vector3 ultimateForce = Vector3.zero;
        float closestDistance = float.MaxValue;

        //if there are humans left
        if(agentScript.humans.Count > 0)
        {
            //for every human, check if it's the closest to the zombie
            for (int i = 0; i < agentScript.humans.Count; i++)
            {
                float distance = Vector3.Distance(gameObject.transform.position, agentScript.humans[i].transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;

                    //if a human is closer than the closestDistance, it becomes the closestHuman
                    closestHuman = agentScript.humans[i];
                }
            }

            //Seek the nearest human
            ultimateForce += Pursue(closestHuman);

            ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxSpeed);

            //make sure the ultimate force does not change the y value
            ultimateForce.y = 0;
            ApplyForce(ultimateForce);

        } else
        {
            //slow them down a small bit
            ApplyFriction(2f);

            //zombie wanders when humans are all gone
            Vector3 wandering = Seek(Wander());
            ApplyForce(wandering);

        }

        //this is SEPERATION
        //run through the zombies list
        for (int i = 0; i < agentScript.zombies.Count; i++)
        {
            //check for the distance on that zombie
            float zombDist = Vector3.Distance(gameObject.transform.position, agentScript.zombies[i].transform.position);

            //if the zombie is within 10 of you and it isn't you, run
            if (gameObject != agentScript.zombies[i])
            {
                if (zombDist < 10)
                {
                    ApplyForce(Flee(agentScript.zombies[i]) * 2);
                }
            }
        }
    }

    //draw debug lines to the targeted human(closest) when the debug lines are enabled
    new void OnRenderObject()
    {
        if (agentScript.enableDebug)
        {
            //grabs forward and right debug lines from Vehicle
            base.OnRenderObject();

            if(closestHuman != null)
            {
                //draws debug line to closesetHuman
                humanMat.SetPass(0);
                GL.Begin(GL.LINES);
                GL.Vertex(transform.position);
                GL.Vertex(closestHuman.transform.position);
                GL.End();
            }

            futureMat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + (velocity * 2));
            GL.End();

        }
    }

}
