using UnityEngine;

public class TileInteraction : MonoBehaviour
{
    private TileManager tileManager;
    private Inventory inventory;
    public float interactionRange = 5f;
    private Transform player;
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        inventory = FindAnyObjectByType<Inventory>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Camera.main == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);

        float distance = Vector2.Distance(player.position, new Vector2(x, y));
        if (distance > interactionRange) return;

        if (Input.GetMouseButtonDown(0))
        {
            tileManager.BreakTile(x, y);
        }
        else if (Input.GetMouseButton(1))
        {
            TileType selectedItem = inventory.getSelectedItem();
            if (selectedItem != TileType.Air)
            {
                bool placed = tileManager.PlaceTile(x, y, selectedItem);
                if (placed) inventory.UseSelectedItem();
            }
        }
    }
}
