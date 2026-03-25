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
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);

        if (Input.GetMouseButtonDown(0))
        {
            tileManager.BreakTile(x, y);
        } else if (Input.GetMouseButton(1))
        {
            tileManager.PlaceTile(x,y);
        }
    }
}
