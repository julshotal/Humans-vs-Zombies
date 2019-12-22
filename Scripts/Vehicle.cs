using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//parent class for the Human and Zombie classes
//calculates regular movement and contains the Seek and Pursue methods
public abstract class Vehicle : MonoBehaviour
{
    // Vectors necessary for force-based movement
    public Vector3 vehiclePosition;
    public Vector3 acceleration;
    public Vector3 direction;
    public Vector3 velocity;

    //wander fields
    Vector3 wanderForce;
    float circleDistance;
    float randomAngle;
    Vector3 circleCenter;

    public Terrain myTerrain;
    public Vector3 terrainSize;
    public Vector3 terrainPOS;

    // Floats
    public float mass;
    public float maxSpeed;

    //y value
    public float defaultY;

    //materials
    public Material forwardMat;
    public Material rightMat;

    //grab sceneManager script
    public GameObject manager;
    public AgentManager agentScript;

    // Start is called before the first frame update
    public void Start()
    {
        //grab terrain data to get the terrain size and position
        myTerrain = Terrain.activeTerrain;
        terrainSize = myTerrain.terrainData.size;
        terrainPOS = myTerrain.transform.position;

        //set vehicle position to the current position
        vehiclePosition = transform.position;

        //find the scenemanager located in scene
        manager = GameObject.Find("Scene Manager");

        //get the scenemanager's script
        agentScript = manager.GetComponent<AgentManager>();
    }

    // Update is called once per frame
    public void Update()
    {
        //do calculations for movement
        velocity += acceleration * Time.deltaTime;
        vehiclePosition += velocity * Time.deltaTime;
        direction = velocity.normalized;
        acceleration = Vector3.zero;
        vehiclePosition.y = defaultY;
        transform.position = vehiclePosition;

        //this is just because Unity was giving me warnings
        //if the direction isn't zero turn that way
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        //call abstract CalcSteeringForces
        CalcSteeringForces();

        //keep everyone in the park
        Bounce();
    }

    // ApplyForce
    // Receive an incoming force, divide by mass, and apply to the cumulative accel vector
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    // SEEK METHOD
    //return Steering force calculated to seek the desired target
    public Vector3 Seek(Vector3 targetPosition)
    {
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        // Step 2: Scale vel to max speed
        // desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate seeking steering force
        Vector3 seekingForce = desiredVelocity - velocity;

        // Step 4: Return force
        return seekingForce;
    }

    // Overloaded Seek
    //returns Steering force calculated to seek the desired target
    public Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }

    // FLEE METHOD
    //return Steering force calculated to flee the desired target
    public Vector3 Flee(Vector3 targetPosition)
    {
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = vehiclePosition - targetPosition;

        // Step 2: Scale vel to max speed
        // desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate seeking steering force
        Vector3 fleeingForce = desiredVelocity - velocity;

        // Step 4: Return force
        return fleeingForce;
    }

    // Overloaded flee
    //returns Steering force calculated to flee the desired target
    public Vector3 Flee(GameObject target)
    {
        return Flee(target.transform.position);
    }

    //EVADE AND PURSUE
    //EVADE the FUTURE positon of a target
    public Vector3 Evade(Vector3 targetPosition)
    {
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = vehiclePosition - targetPosition;

        // Step 2: Scale vel to max speed
        // desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        // Step 3:  Calculate evading steering force
        Vector3 evadingForce = desiredVelocity - velocity;

        // Step 4: Return force
        return evadingForce;
    }

    // Overloaded Evade
    //returns Steering force calculated to evade the desired target
    public Vector3 Evade(GameObject target)
    {
        //grab this script to get the velocity
        Vehicle targScript = target.GetComponent<Vehicle>();

        //this seeks the future positon rather than current position
        return Evade(target.transform.position + (targScript.velocity * 2));
    }

    //PURSUE the FUTURE positon of a target
    public Vector3 Pursue(Vector3 targetPosition)
    {
        // Step 1: Find DV (desired velocity)
        // TargetPos - CurrentPos
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        // Step 2: Scale vel to max speed
        // desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;


        // Step 3:  Calculate seeking steering force
        Vector3 pursueForce = desiredVelocity - velocity;

        // Step 4: Return force
        return pursueForce;
    }

    // Overloaded pursue
    //returns Steering force calculated to pursue the desired target
    public Vector3 Pursue(GameObject target)
    {
        //grab this script to get the velocity
        Vehicle targScript = target.GetComponent<Vehicle>();
        //this seeks the future positon rather than current position
        return Pursue(target.transform.position + (targScript.velocity * 2));
    }

    //Apply friction to an object
    public Vector3 ApplyFriction(float coeff)
    {
        //friction is applied to the acceleration to slow down the vehicle
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        acceleration += friction;

        return acceleration;
    }

    //abstract method for the human and zombie steering forces, will be overrode in the children classes
    public abstract void CalcSteeringForces();

    //gameobjects wander aimlessly around the terrain
    //this replaces the PSG the humans sought
    public Vector3 Wander()
    {
        //set a circle in front of the object
        circleDistance = 10;

        //circle's center is equal to velocity
        circleCenter = velocity;

        //grab a random angle within that circles
        randomAngle = Random.Range(-360, 360);

        //normalize vector and scale by distance
        circleCenter.Normalize();
        circleCenter *= circleDistance;

        //find Cos and Sin of the angle for X and Z positions and multiply by the radius of 5
        float X = Mathf.Cos(randomAngle) * 5;
        float Z = Mathf.Sin(randomAngle) * 5;

        //calculate the vector of the vehicle positon and the circle and add that to the calculate X and Z
        //this is returned as our wandering forces
        wanderForce = transform.position + (transform.forward * circleDistance) + new Vector3(X, 0, Z);

        return wanderForce;
    }

    //detects world boundaries to keep objects in the park
    void Bounce()
    {

        if (vehiclePosition.x > terrainPOS.x + terrainSize.x - 5)
        {
            //when objects move outside the park, they turn and seek the center until they are away from the edges
            ApplyForce(Seek(new Vector3(terrainSize.x/2, 1, terrainSize.y/2)) * 10);
        }
        else if (vehiclePosition.x < terrainPOS.x + 5)
        {
            ApplyForce(Seek(new Vector3(terrainSize.x / 2, 1, terrainSize.y / 2)) * 10);
        }

        if (vehiclePosition.z > terrainPOS.z + terrainSize.z - 5)
        {
            ApplyForce(Seek(new Vector3(terrainSize.x / 2, 1, terrainSize.y / 2)) * 10);
        }
        else if (vehiclePosition.z < terrainPOS.z + 5)
        {
            ApplyForce(Seek(new Vector3(terrainSize.x / 2, 1, terrainSize.y / 2)) * 10);
        }

    }

    //Debug lines for forward/right movements
    public void OnRenderObject()
    {
        //if debuglines are toggled on
        if (agentScript.enableDebug) {

            //draw the forward vector for each vehicle
            forwardMat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex((transform.forward * 2) + transform.position);
            GL.End();

            //draw the right vector for each vehicle
            rightMat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(transform.position);
            GL.Vertex((transform.right * 2) + transform.position);
            GL.End();

        }

    }

    public Vector3 ObstacleAvoidance(float vehicleRadius, float safeSpace, GameObject obstacle)
    {
        Vector3 desiredVelocity;

        //grab the obstacle script
        //im aware the obstacle script is a little useless but Erin told us to do it
        Obstacle obScript = obstacle.GetComponent<Obstacle>();

        //grab vector to center between obstacle and gameobject
        Vector3 vectorTOC = obstacle.transform.position - gameObject.transform.position;

        //dot products of the forward and right transforms 
        float forwardProduct = Vector3.Dot(vectorTOC, gameObject.transform.forward);
        float rightProduct = Vector3.Dot(vectorTOC, gameObject.transform.right);

        //check if the objects are behind and if so, exit
        if (forwardProduct < 0)
        {
            return Vector3.zero;
        }

        //check if the obstacles is outside the safespace
        if (vectorTOC.magnitude > safeSpace)
        {
            return Vector3.zero;
        }

        // If dot right is > radii sum, exit method
        if (obScript.radius + vehicleRadius < Mathf.Abs(rightProduct))
        {
            return Vector3.zero;
        }

        if (rightProduct < 0)        // turn right, obstacle is to the left
        {
            desiredVelocity = transform.right;
        }
        else                    // turn left, obstacle is to the right
        {
            desiredVelocity = -transform.right;
        }

        //return the steering force
        return desiredVelocity;

    }

    //GUI telling users what key to press to toggle debug lines and change cameras
    void OnGUI()
    {
        GUI.color = Color.cyan;
        GUI.skin.box.fontSize = 20;
        GUI.Box(new Rect(10, 10, 300, 80), "Press 'd' to toggle debug lines \n Press 'c' to change camera view");
        GUI.skin.box.wordWrap = true;
    }

}
