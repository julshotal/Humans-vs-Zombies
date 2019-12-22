using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for the scene manager to run instantiation and collision
public class AgentManager : MonoBehaviour
{
    //grab gameobjects
    public GameObject human;
    public GameObject zombie;
    public GameObject obstacle;
    public GameObject newObstacle;

    //create lists to hold all humans, all zombies, and the dead humans
    public List<GameObject> humans = new List<GameObject>();
    public List<GameObject> zombies = new List<GameObject>();
    public List<GameObject> dead = new List<GameObject>();
    public List<GameObject> obstacles = new List<GameObject>();

    //grab terrain variabes
    public Terrain myTerrain;
    public Vector3 terrainSize;
    public Vector3 terrainPOS;

    //variables to instantiate the first zombie, and new zombies from bitten humans
    public GameObject enemy;
    GameObject newbie;

    //variables to run collision, a wait is added so collision won't trigger multiple times on one human
    public Collisions runCollision;
    public float collisionWait = 1f;
    public float timer = 0;

    //boolean for button click
    public bool enableDebug;

    //vars to track amount of humans and zombies
    float numHuman;
    float numZombie;

    // Start is called before the first frame update
    void Start()
    {

        //intialize debug lines to false
        enableDebug = false;

        //grabb terrain data to get the terrain size and position
        myTerrain = Terrain.activeTerrain;
        terrainSize = myTerrain.terrainData.size;
        terrainPOS = myTerrain.transform.position;

        //create 15 new humans, each is added to the humans list
        for (int i = 0; i < 15; i++)
        {
            GameObject newHuman = Instantiate(human, new Vector3(Random.Range(terrainPOS.x, terrainPOS.x + terrainSize.x), 1f, Random.Range(terrainPOS.z, terrainPOS.z + terrainSize.z)), Quaternion.identity);
            humans.Add(newHuman);
        }

        //istantiate obstacles
        for(int i = 0; i < 10; i++)
        {
           newObstacle = Instantiate(obstacle, new Vector3(Random.Range(terrainPOS.x + 5, (terrainPOS.x + terrainSize.x) - 5), -0.01f, Random.Range(terrainPOS.z + 5, (terrainPOS.z + terrainSize.z) - 5)), Quaternion.identity);
           obstacles.Add(newObstacle);
        }

        //create the first zombie and add it to the zombie list
        enemy = Instantiate(zombie, new Vector3(Random.Range(terrainPOS.x, terrainPOS.x + terrainSize.x), 1.5f, Random.Range(terrainPOS.z, terrainPOS.z + terrainSize.z)), Quaternion.identity);
        zombies.Add(enemy);

        //grab the collision script
        runCollision = GetComponent<Collisions>();
    }

    // Update is called once per frame
    void Update()
    {
        //if 'd' key is pressed, the debug line boolean is toggled
        if (Input.GetKeyUp(KeyCode.D))
        {
            enableDebug = !enableDebug;
        }

        //update total number of stored humans and zombies
        numHuman = humans.Count;
        numZombie = zombies.Count;
    }

    //method which removes deceased humans from the humans list and then destroys their gameobject
    public void RemoveHumans()
    {
        foreach(GameObject deceased in dead)
        {
            humans.Remove(deceased);
        }

        for(int i = 0; i < dead.Count; i++)
        {
            Destroy(dead[i]);

            //istantiate new zombie at humans position and add to the zombies list
            newbie = Instantiate(zombie, new Vector3(dead[i].transform.position.x, 1, dead[i].transform.position.z), Quaternion.identity);
            zombies.Add(newbie);
        }

        dead.Clear();

    }

    //use circle collision to check collision between humans and zombies
    public void CheckCollision()
    {
        //add deltatime to the timer
        timer += timer += Time.deltaTime;

        //if there are humans left and the timer is up, run collision
        if (timer > collisionWait)
        {
            //run through the humans and zombies list
            foreach (GameObject person in humans)
            {
                for (int i = 0; i < zombies.Count; i++)
                {
                    //if the human and zombie are colliding
                    if (runCollision.CircleCollision(person, zombies[i]))
                    {
                        //reset timer to 0
                        timer = 0;

                        //add the deceased human to dead list
                        dead.Add(person);             
                    }
                }
            }
        }
    }

    //creates two buttons to create new humans and zombies
    private void OnGUI()
    {
        GUI.skin.button.fontSize = 20;

        GUI.color = Color.cyan;
        if (GUI.Button(new Rect(10, 150, 140, 50), "Add Human"))
        {
            //only can have up to 30 humans at a time
            if(humans.Count < 30)
            {
                GameObject addHuman = Instantiate(human, new Vector3(Random.Range(terrainPOS.x, terrainPOS.x + terrainSize.x), 1f, Random.Range(terrainPOS.z, terrainPOS.z + terrainSize.z)), Quaternion.identity);
                humans.Add(addHuman);
            }
        }

        if (GUI.Button(new Rect(170, 150, 140, 50), "Add Zombie"))
        {
            //arbitrary number that limits amount of addable zombies to a more reasonable amount
            if(zombies.Count < 16)
            {
                GameObject addZomb = Instantiate(zombie, new Vector3(Random.Range(terrainPOS.x, terrainPOS.x + terrainSize.x), 1.5f, Random.Range(terrainPOS.z, terrainPOS.z + terrainSize.z)), Quaternion.identity);
                zombies.Add(addZomb);
            }
        }

        GUI.skin.label.fontSize = 15;
        GUI.color = Color.black;

        //displays total number of humans and zommbies in the scene
        GUI.Label(new Rect(15, 210, 140, 50), "Humans: " + numHuman);
        GUI.Label(new Rect(175, 210, 140, 50), "Zombies: " + numZombie);
    }
}
