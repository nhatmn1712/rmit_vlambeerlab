using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Generation1;
    public GameObject Generation2;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;    
        }
        else
        {
            Debug.LogError("Thre is more than 1 GameManager.cs");
        }
    }

    private void Start()
    {
        Generate1();
    }

    public void Generate1()
    {
        Generation1.SetActive(false);
        Generation2.SetActive(false);

        ClearDungeon();
        Generation1.SetActive(true);
        Generation2.SetActive(false);
    }

    public void Generate2()
    {
        Generation1.SetActive(false);
        Generation2.SetActive(false);

        ClearDungeon();
        Generation1.SetActive(false);
        Generation2.SetActive(true);
    }

    public void ClearDungeon()
    {
        if (CountDown.Done)
        {
            CountDown.counter = 0;
            CountDown.Done = false;
            CountDown.placeWall = false;

            foreach (var generatedObject in CountDown.generatedObjects)
            {
                Destroy(generatedObject);
            }

            foreach (var wallObject in CountDown.wallObjects)
            {
                Destroy(wallObject);
            }

            foreach (var pathmakerSphere in CountDown.pathmakerSpheres)
            {
                Destroy(pathmakerSphere);
            }

            CountDown.generatedObjects.Clear();
            CountDown.pathmakerSpheres.Clear();

            // SelectRandomFloorTile();
        }
    }
}