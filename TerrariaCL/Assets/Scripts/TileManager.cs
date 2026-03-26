using UnityEngine;
using UnityEngine.Tilemaps;

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
    public float noiseScale = 0.02f;
    public float seed;

    public Tilemap tilemap;
    public RuleTile dirtTile;
    public RuleTile stoneTile;
    public float caveScale = 0.05f;
    public float caveThreshold = 0.45f;
    private TileType[,] tiles;

    void Start()
    {
        GenerateWorld();
        RenderWorld();
    }

    void GenerateWorld()
    {
        seed = Random.Range(0f, 10000f);
        tiles = new TileType[worldWidth, worldHeight];

        for (int x = 0; x < worldWidth; x++)
        {
            int surfaceHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x * noiseScale + seed, 0) * worldHeight * 0.3f + worldHeight * 0.4f);

            for (int y = 0; y < worldHeight; y++)
            {
                if (y > surfaceHeight)
                    tiles[x, y] = TileType.Air;
                else if (y > surfaceHeight - 3)
                {
                    tiles[x, y] = TileType.Dirt;
                }
                else if (y > surfaceHeight - 25)
                {
                    float transitionNoise = Mathf.PerlinNoise(x * 0.2f + seed, y * 0.2f + seed);
                    float transitionChance = (float)(y - (surfaceHeight - 25)) / 22f;
                    tiles[x, y] = transitionNoise > transitionChance ? TileType.Stone : TileType.Dirt;
                }
                else
                {
                    float caveNoise = Mathf.PerlinNoise(x * caveScale + seed, y * caveScale + seed * 0.5f);
                    if (caveNoise < caveThreshold)
                        tiles[x, y] = TileType.Air;
                    else
                        tiles[x, y] = TileType.Stone;
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
                TileBase tile = GetTileAsset(tiles[x, y]);
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    TileBase GetTileAsset(TileType type)
    {
        switch (type)
        {
            case TileType.Dirt: return dirtTile;
            case TileType.Stone: return stoneTile;
            default: return null;
        }
    }

    public void BreakTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return;
        if (tiles[x, y] == TileType.Air)
            return;

        tiles[x, y] = TileType.Air;
        tilemap.SetTile(new Vector3Int(x, y, 0), null);
    }

    public void PlaceTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return;
        if (tiles[x, y] != TileType.Air)
            return;

        tiles[x, y] = TileType.Dirt;
        tilemap.SetTile(new Vector3Int(x, y, 0), dirtTile);
    }
}