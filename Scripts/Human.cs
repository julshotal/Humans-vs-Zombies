using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the human class must seek the purple target while fleeing zombies that approach
public class Human : Vehicle
{
    public Material futureMat;

    // Start is called before the first frame update
    public new void Start()
    {
        //grab the vehicle start() method
        base.Start();
    }

    public new void Update()
    {
        base.Update();

        //make sure y is always 1
        defaultY = 1f;

        //avoid all obstacles
        foreach (GameObject obstacle in agentScript.obstacles)
        {
            ApplyForce(ObstacleAvoidance(5, 15, obstacle) * 4f);
        }
    }

    //override the abstract steering forces method to seek the target and flee zombies
    public override void CalcSteeringForces()
    {
        //for all the zombies on the screen
        for(int i = 0; i < agentScript.zombies.Count; i++)
        {
            //reset ultimate force
            Vector3 ultimateForce = Vector3.zero;

            //get the distance between the human and zombies AND the distance between the human and it's target (PSG)
            float distance = Vector3.Distance(gameObject.transform.position, agentScript.zombies[i].transform.position);

            //if the zombie is far away, friction is applied to slow the human
            if (distance > 10)
            {
                ApplyFriction(5f);

                //human wanders when not threatened
                Vector3 wandering = Seek(Wander());
                ApplyForce(wandering);

                //if the zombie is nearby, the human begins to evade (the flee response is slightly stronger than the wandering response)
            }
            else if (distance < 10)
            {
                ultimateForce += (Evade(agentScript.zombies[i]) * 5f);

                //apply ultimate force to the human, make sure y doesn't change
                ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxSpeed);
                ultimateForce.y = 0;
                ApplyForce(ultimateForce);
            }

        }

        //this is SEPERATION
        //run through the humans list
        for (int i = 0; i < agentScript.humans.Count; i++)
        {
            //check for the distance on that human
            float humanDist = Vector3.Distance(gameObject.transform.position, agentScript.humans[i].transform.position);

            //if the human is within 5 of you and it isn't you, run
            if (gameObject != agentScript.humans[i])
            {
                if(humanDist < 5)
                {
                    ApplyForce(Flee(agentScript.humans[i]) * 2);
                }
            }
        }
    }

    new void OnRenderObject()
    {
        if (agentScript.enableDebug)
        {
            //grabs forward and right debug lines from Vehicle
            base.OnRenderObject();

            futureMat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + (velocity * 2));
            GL.End();

        }
    }
}
