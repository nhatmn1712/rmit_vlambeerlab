using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public float waterLevel = .4f;
    public GameObject Prefab_Grass;
    public GameObject Prefab_Water;
    public GameObject[] treePreFabs;
    public float scale = .1f;
    public float treeNoiseScale = .05f;
    public float treeDesity = .5f;
    public int size = 200;

    Cell[,] grid;

    void OnEnable()
    {
        grid = new Cell[size, size];
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));


        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }


        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }


        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y] - falloffMap[x, y];
                bool isWater = noiseValue < waterLevel;

                Cell cell = new Cell(isWater);
                grid[x, y] = cell;

                GameObject cube = null;
                if (cell.isWater)
                {
                    cube = Instantiate(Prefab_Water);
                }
                else
                {
                    cube = Instantiate(Prefab_Grass);
                }

                cube.transform.position = new Vector3(x, 0, y);
                cube.transform.localScale = Vector3.one;
                cube.transform.SetParent(GameManager.Instance.Generation2.transform);
                CountDown.generatedObjects.Add(cube);
            }
        }

        Camera.main.transform.localPosition = new Vector3(45.5f, 21.1f, 24.8f);
        Camera.main.transform.localEulerAngles = new Vector3(52.3f, 0, 0);
        CountDown.Done = true;
        GenerateTrees(grid);
    }

    void GenerateTrees(Cell[,] grid)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (!cell.isWater)
                {
                    float v = Random.Range(0f, treeDesity);
                    if (noiseMap[x, y] < v)
                    {
                        GameObject prefab = treePreFabs[Random.Range(0, treePreFabs.Length)];
                        GameObject tree = Instantiate(prefab, transform.position, Quaternion.identity, GameManager.Instance.Generation2.transform);
                        CountDown.generatedObjects.Add(tree);
                        tree.transform.position = new Vector3(x, .5f, y);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                    }
                }
            }
        }
    }
}