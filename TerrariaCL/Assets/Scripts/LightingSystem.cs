using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LightingSystem : MonoBehaviour
{
    public TileManager tileManager;
    public Tilemap tilemap;
    public Camera cam;

    public float skyLight = 1.0f;
    public float airFalloff = 0.02f;
    public float solidFalloff = 0.08f;
    public float minLight = 0.05f;
    public int bufferSize = 20;

    private float[,] lightMap;
    private int worldWidth;
    private int worldHeight;
    private Vector3 lastCamPos;

    void Start()
    {
        worldWidth = tileManager.worldWidth;
        worldHeight = tileManager.worldHeight;
        lightMap = new float[worldWidth, worldHeight];
    }

    void Update()
    {
        if (Vector3.Distance(cam.transform.position, lastCamPos) > 2f)
        {
            lastCamPos = cam.transform.position;
            CalculateLighting();
        }
    }

    public void CalculateLighting()
    {
        Vector3 camPos = cam.transform.position;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        int minX = Mathf.Max(0, Mathf.FloorToInt(camPos.x - camWidth) - bufferSize);
        int maxX = Mathf.Min(worldWidth - 1, Mathf.CeilToInt(camPos.x + camWidth) + bufferSize);
        int minY = Mathf.Max(0, Mathf.FloorToInt(camPos.y - camHeight) - bufferSize);
        int maxY = Mathf.Min(worldHeight - 1, Mathf.CeilToInt(camPos.y + camHeight) + bufferSize);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
                lightMap[x, y] = minLight;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // seed light from surface downward through air
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = maxY; y >= minY; y--)
            {
                lightMap[x, y] = skyLight;
                queue.Enqueue(new Vector2Int(x, y));
                if (tileManager.GetTile(x, y) != TileType.Air)
                    break;
            }
        }

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            float currentLight = lightMap[current.x, current.y];

            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                if (nx < minX || nx > maxX || ny < minY || ny > maxY)
                    continue;

                TileType neighborType = tileManager.GetTile(nx, ny);
                float falloff = neighborType == TileType.Air ? airFalloff : solidFalloff;
                float newLight = currentLight - falloff;

                if (newLight > lightMap[nx, ny])
                {
                    lightMap[nx, ny] = newLight;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        ApplyLighting(minX, maxX, minY, maxY);
    }

    void ApplyLighting(int minX, int maxX, int minY, int maxY)
    {
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float light = Mathf.Clamp(lightMap[x, y], minLight, 1f);
                Color color = new Color(light, light, light, 1f);
                tilemap.SetColor(new Vector3Int(x, y, 0), color);
            }
        }
    }
}