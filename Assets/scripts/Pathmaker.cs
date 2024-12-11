using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class CountDown
{
    public static int counter;
    public static bool Done;

    public static GameObject randomTile;
    public static List<GameObject> generatedObjects = new List<GameObject>();
    public static List<GameObject> pathmakerSpheres = new List<GameObject>();
}

public class Pathmaker : MonoBehaviour
{
    public bool isOrigin;

    public Transform[] floorTilePrefabs;

    public Transform pathmakerSpherePrefab;

    public GameObject wallPrefab;  // Reference to the wall prefab


    void Start()
    {
        // Randomly select one floor prefab from the array at the start
        if (isOrigin)
            SelectRandomFloorTile();
    }

    void Update()
    {
        //		If counter is less than 50, then:
        if (CountDown.counter < 150)
        {
            float randomNumber = Random.Range(0.0f, 1.0f);

            if (randomNumber < 0.25f)
            {
                transform.localEulerAngles += new Vector3(0f, 90f, 0f);
            }

            else if (randomNumber >= 0.25f && randomNumber <= 0.5f)
            {
                transform.localEulerAngles += new Vector3(0f, -90f, 0f);
            }

            else if (randomNumber >= 0.95f && randomNumber <= 1.0f)
            {
                GameObject pathmaker = Instantiate(pathmakerSpherePrefab, transform.position, Quaternion.identity).gameObject;
                pathmaker.GetComponent<Pathmaker>().isOrigin = false;
                CountDown.pathmakerSpheres.Add(pathmaker);
            }
            else
            {
                randomNumber = Random.Range(0.0f, 1.0f);
                if (randomNumber < 0.75f)
                {
                    for (int i = 0; i < Random.Range(5, 10); i++)
                    {
                        CountDown.generatedObjects.Add(Instantiate(CountDown.randomTile, transform.position, Quaternion.identity).gameObject);
                        UpdateCamera();
                        transform.position += transform.forward * 1.0f;
                    }
                }
                else
                {
                    Vector3 oldPosition = transform.position;
                    int space = Random.Range(4, 8);
                    for (int x = 0; x < space; x++)
                    {
                        for (int y = 0; y < space; y++)
                        {
                            Vector3 newPosition = new Vector3(oldPosition.x + x, oldPosition.y, oldPosition.z + y);
                            CountDown.generatedObjects.Add(Instantiate(CountDown.randomTile, newPosition, Quaternion.identity).gameObject);
                            UpdateCamera();
                            transform.position = newPosition;
                        }
                    }
                }

                CountDown.counter++;



            }
        }

        else
        {
            gameObject.SetActive(isOrigin);

        }
        if (CountDown.counter == 150)
        {
            CountDown.Done = true;

            Debug.Log("Done");
            if (CountDown.Done && Input.GetKey(KeyCode.X))
            {
                Debug.Log("Pressed");
                var sceneName = SceneManager.GetActiveScene();
                SceneManager.LoadScene(sceneName.name);

                CountDown.counter = 0;
                CountDown.Done = false;

                foreach (var generatedObject in CountDown.generatedObjects)
                {
                    Destroy(generatedObject);
                }

                foreach (var pathmakerSphere in CountDown.pathmakerSpheres)
                {
                    Destroy(pathmakerSphere);
                }

                CountDown.generatedObjects.Clear();
                CountDown.pathmakerSpheres.Clear();

                SelectRandomFloorTile();
            }
        }

    }

    void SelectRandomFloorTile()
    {
        int randomIndex = Random.Range(0, floorTilePrefabs.Length);
        CountDown.randomTile = floorTilePrefabs[randomIndex].gameObject;
    }
    void PlaceWallsAroundFloor(Vector3 floorPosition)
    {
        // Check if there is a void around the current floor position and place walls accordingly.
        if (IsVoid(floorPosition + Vector3.up)) InstantiateWall(floorPosition + Vector3.up);  // North
        if (IsVoid(floorPosition + Vector3.down)) InstantiateWall(floorPosition + Vector3.down); // South
        if (IsVoid(floorPosition + Vector3.right)) InstantiateWall(floorPosition + Vector3.right); // East
        if (IsVoid(floorPosition + Vector3.left)) InstantiateWall(floorPosition + Vector3.left);  // West
    }

    bool IsVoid(Vector3 position)
    {
        // Perform a raycast or check if there's an existing floor tile at the position
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 1f))
        {
            return hit.collider == null; // No floor tile means it's a void
        }
        return true;
    }

    void InstantiateWall(Vector3 position)
    {
        Instantiate(wallPrefab, position, Quaternion.identity);
    }

    public void UpdateCamera()
    {
        Vector3 averagePos = Vector3.zero;
        foreach (var generatedObject in CountDown.generatedObjects)
        {
            averagePos += generatedObject.transform.localPosition;
        }
        averagePos /= CountDown.generatedObjects.Count;

        Camera.main.transform.position = new Vector3(averagePos.x, Camera.main.transform.position.y, averagePos.z);
    }
}



// STEP 6:  =====================================================================================
// art pass, usability pass

// - move the game camera to a position high in the world, and then point it down, so we can see your world get generated
// - CHANGE THE DEFAULT UNITY COLORS
// - add more detail to your original floorTile placeholder -- and let it randomly pick one of 3 different floorTile models, etc. so for example, it could randomly pick a "normal" floor tile, or a cactus, or a rock, or a skull
// - or... make large city tiles and create a city.  Set the camera low so and une the values so the city tiles get clustered tightly together.

//		- MODEL 3 DIFFERENT TILES IN BLENDER.  CREATE SOMETHING FROM THE DEEP DEPTHS OF YOUR MIND TO PROCEDURALLY GENERATE. 
//		- THESE TILES CAN BE BASED ON PAST MODELS YOU'VE MADE, OR NEW.  BUT THEY NEED TO BE UNIQUE TO THIS PROJECT AND CLEARLY TILE-ABLE.

//		- then, add a simple in-game restart button; let us press [R] to reload the scene and see a new level generation
// - with Text UI, name your proc generation system ("AwesomeGen", "RobertGen", etc.) and display Text UI that tells us we can press [R]


// EXTRA TASKS TO DO, IF YOU WANT / DARE: ===================================================

// AVOID SPAWNING A TILE IN THE SAME PLACE AS ANOTHER TILE  https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
// Check out the Physics.OverlapSphere functionality... 
//     If the collider is overlapping any others (the tile prefab has one), prevent a new tile from spawning and move forward one space. 

// DYNAMIC CAMERA:
// position the camera to center itself based on your generated world...
// 1. keep a list of all your spawned tiles
// 2. then calculate the average position of all of them (use a for() loop to go through the whole list) 
// 3. then move your camera to that averaged center and make sure fieldOfView is wide enough?

// BETTER UI:
// learn how to use UI Sliders (https://unity3d.com/learn/tutorials/topics/user-interface-ui/ui-slider) 
// let us tweak various parameters and settings of our tech demo
// let us click a UI Button to reload the scene, so we don't even need the keyboard anymore.  Throw that thing out!

// WALL GENERATION
// add a "wall pass" to your proc gen after it generates all the floors
// 1. raycast out from each floor tile (that'd be 4 raycasts per floor tile, in a square "ring" around each tile?)
// 2. if the raycast "fails" that means there's empty void there, so then instantiate a Wall tile prefab
// 3. ... repeat until walls surround your entire floorplan
// (technically, you will end up raycasting the same spot over and over... but the "proper" way to do this would involve keeping more lists and arrays to track all this data)