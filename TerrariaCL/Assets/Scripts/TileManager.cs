using UnityEngine;

public enum TileType
{
    Air = 0,
    Dirt = 1,
    Stone = 2
}

public class TileManager : MonoBehaviour
{
    public float seed;
    public int worldWidth = 100;
    public int worldHeight = 50;
    private TileType[,] tiles;
    private GameObject[,] tileObjects;
    public float noiseScale = 0.1f;

    void Start()
    {
        GenerateWorld();
        RenderWorld();
    }

    void GenerateWorld()
    {
        seed = Random.Range(0f, 1000f);
        tiles = new TileType[worldWidth, worldHeight];
        tileObjects = new GameObject[worldWidth, worldHeight];

        for (int x = 0; x < worldWidth; x++)
        {
            int surfaceHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x * noiseScale + seed, 0) * worldHeight * 0.5f + worldHeight * 0.25f);

            for (int y = 0; y < worldHeight; y++)
            {
                if (y > surfaceHeight)
                {
                    tiles[x, y] = TileType.Air;
                }
                else if (y > surfaceHeight - 10)
                {
                    tiles[x, y] = TileType.Dirt; // solid
                }
                else
                {
                    tiles[x, y] = TileType.Stone; // air
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
                if (tiles[x, y] == TileType.Air)
                {
                    continue;
                }

                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.position = new Vector3(x, y, 0);
                tileObjects[x, y] = tile;

                Color tileColor = GetTileColor(tiles[x, y]);
                Renderer renderer = tile.GetComponent<Renderer>();
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = tileColor;
                renderer.material = mat;
            }
        }
    }

    Color GetTileColor(TileType type)
    {
        switch (type)
        {
            case TileType.Dirt:
                return new Color(0.6f, 0.4f, 0.2f);
            case TileType.Stone:
                return new Color(0.5f, 0.5f, 0.5f);
            default:
                return Color.white;
        }
    }

    public void BreakTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return;
        if (tiles[x, y] == TileType.Air)
            return;

        tiles[x, y] = TileType.Air;
        UpdateTileVisual(x, y);
    }

    void UpdateTileVisual(int x, int y)
    {
        if (tileObjects[x, y] != null)
        {
            Destroy(tileObjects[x, y]);
            tileObjects[x, y] = null;
        }
        else
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tile.transform.position = new Vector3(x, y, 0);
            tileObjects[x, y] = tile;

            Color tileColor = GetTileColor(tiles[x, y]);
            Renderer renderer = tile.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = tileColor;
            renderer.material = mat;
        }
    }

    internal void PlaceTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return;
        if (tiles[x, y] == TileType.Stone || tiles[x, y] == TileType.Dirt)
            return;

        tiles[x, y] = TileType.Dirt;
        UpdateTileVisual(x, y);
    }
}