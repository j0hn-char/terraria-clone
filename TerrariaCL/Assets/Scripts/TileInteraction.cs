using UnityEngine;

public class TileInteraction : MonoBehaviour
{
    private TileManager tileManager;
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
    }

    void Update()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.y);

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Clicked at tile at X:{x} Y:{y}");
            tileManager.BreakTile(x, y);
        } else if (Input.GetMouseButton(1))
        {
            tileManager.PlaceTile(x,y);
        }
    }
}
