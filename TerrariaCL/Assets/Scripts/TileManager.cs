using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int worldWidth = 100;
    public int worldHeight = 50;

    private int[,] tiles;

    void Start()
    {
        GenerateWorld();
        RenderWorld();
    }

    void GenerateWorld()
    {
        tiles = new int[worldWidth, worldHeight];

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (y < worldHeight / 2)
                {
                    tiles[x, y] = 1; // solid
                }
                else
                {
                    tiles[x, y] = 0; // air
                }
            }
        }
    }

    void RenderWorld()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (tiles[x, y] == 1)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tile.transform.position = new Vector3(x, y, 0);
                }
            }
        }
    }
}