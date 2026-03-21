using UnityEngine;

public enum TileType
{
    Air = 0,
    Dirt = 1,
    Stone = 2
}

public class TileManager : MonoBehaviour
{
    public int worldWidth = 100;
    public int worldHeight = 50;

    private TileType[,] tiles;

    void Start()
    {
        GenerateWorld();
        RenderWorld();
    }

    void GenerateWorld()
    {
        tiles = new TileType[worldWidth, worldHeight];

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (y < worldHeight / 4)
                {
                    tiles[x, y] = TileType.Stone;
                }
                else if (y < worldHeight / 2)
                {
                    tiles[x, y] = TileType.Dirt; // solid
                }
                else
                {
                    tiles[x, y] = TileType.Air; // air
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
}