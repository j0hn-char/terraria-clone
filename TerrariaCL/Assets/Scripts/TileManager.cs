using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public enum TileType
{
    Air = 0,
    Dirt = 1,
    Stone = 2,
    Iron = 3,
    Bronze = 4,
    Titanium = 5
}

public class TileManager : MonoBehaviour
{
    public int worldWidth = 100;
    public int worldHeight = 50;
    public float noiseScale = 0.02f;
    public float seed;

    public GameObject itemDropPrefab;
    public Tilemap tilemap;
    public RuleTile dirtTile;
    public RuleTile stoneTile;
    public RuleTile ironTile;
    public RuleTile bronzeTile;
    public RuleTile titaniumTile;
    public float caveScale = 0.05f;
    public float caveThreshold = 0.45f;
    private TileType[,] tiles;
    private Inventory inventory;
    private LightingSystem lightingSystem;

    void Start()
    {
        lightingSystem = FindObjectOfType<LightingSystem>();
        inventory = FindObjectOfType<Inventory>();
        GenerateWorld();
        RenderWorld();
        if (lightingSystem != null)
            StartCoroutine(ApplyLightingDelayed());
    }

    IEnumerator ApplyLightingDelayed()
    {
        yield return null;
        lightingSystem.CalculateLighting();
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
                    {
                        tiles[x, y] = TileType.Air;
                    }
                    else
                    {
                        float ironNoise = Mathf.PerlinNoise(x * 0.1f + seed * 1.5f, y * 0.1f + seed * 1.5f);
                        float bronzeNoise = Mathf.PerlinNoise(x * 0.08f + seed * 2.5f, y * 0.08f + seed * 2.5f);
                        float titaniumNoise = Mathf.PerlinNoise(x * 0.06f + seed * 3.5f, y * 0.06f + seed * 3.5f);

                        float depthFactor = 1f - ((float)y / worldHeight);

                        if (titaniumNoise > 0.78f && depthFactor > 0.6f)
                            tiles[x, y] = TileType.Titanium;
                        else if (bronzeNoise > 0.72f && depthFactor > 0.4f)
                            tiles[x, y] = TileType.Bronze;
                        else if (ironNoise > 0.65f)
                            tiles[x, y] = TileType.Iron;
                        else
                            tiles[x, y] = TileType.Stone;
                    }
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
                Vector3Int pos = new Vector3Int(x, y, 0);
                tilemap.SetTile(pos, GetTileAsset(tiles[x, y]));
                tilemap.SetTileFlags(pos, TileFlags.None);
                tilemap.SetColor(pos, Color.black);
            }
        }
    }

    TileBase GetTileAsset(TileType type)
    {
        switch (type)
        {
            case TileType.Dirt: return dirtTile;
            case TileType.Stone: return stoneTile;
            case TileType.Iron: return ironTile;
            case TileType.Bronze: return bronzeTile;
            case TileType.Titanium: return titaniumTile;
            default: return null;
        }
    }

    public void BreakTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return;
        if (tiles[x, y] == TileType.Air)
            return;
        TileType previousType = tiles[x, y];
        tiles[x, y] = TileType.Air;
        tilemap.SetTile(new Vector3Int(x, y, 0), null);
        if (itemDropPrefab != null)
        {
            GameObject drop = Instantiate(itemDropPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
            ItemDrop itemDrop = drop.GetComponent<ItemDrop>();
            itemDrop.itemType = previousType;
        }
        if (lightingSystem != null)
            lightingSystem.CalculateLighting();
    }

    public bool PlaceTile(int x, int y, TileType type)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return false;
        if (tiles[x, y] != TileType.Air)
            return false;

        tiles[x, y] = type;
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, GetTileAsset(type));
        tilemap.SetTileFlags(pos, TileFlags.None);
        if (lightingSystem != null)
            lightingSystem.CalculateLighting();
        return true;
    }

    public TileType GetTile(int x, int y)
    {
        if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
            return TileType.Stone;
        return tiles[x, y];
    }
}