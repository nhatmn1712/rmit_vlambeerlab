using UnityEngine;


public class WorldGenerator : MonoBehaviour
{
    public float waterLevel = .4f;
    public float scale = .1f;
    public int size = 200;

    Cell[,] grid;

    void Start()
    {
        // Generate noise map
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

        // Generate falloff map
        float[,] faloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                faloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        // Initialize grid and create cubes
        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= faloffMap[x, y];
                bool isWater = noiseValue < waterLevel;

                // Create Cell
                Cell cell = new Cell(isWater);
                grid[x, y] = cell;

                // Create GameObject (cube) for visualization
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, 0, y);
                cube.transform.localScale = Vector3.one;

                // Set cube color based on whether it's water or land
                Renderer renderer = cube.GetComponent<Renderer>();
                if (cell.isWater)
                {
                    renderer.material.color = Color.blue;
                }
                else
                {
                    renderer.material.color = Color.green;
                }
            }
        }
    }
}