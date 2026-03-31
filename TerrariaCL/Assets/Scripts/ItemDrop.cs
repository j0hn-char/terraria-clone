using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public TileType itemType;
    public float pickupRange = 2f;
    public float attractRange = 3f;
    public float attractSpeed = 8f;

    private Transform player;
    private Rigidbody2D rb;
    private float bobTimer;
    private bool attracted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < pickupRange)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            inventory.AddItem(itemType);
            Destroy(gameObject);
            return;
        }

        if (distance < attractRange)
        {
            attracted = true;
        }

        if (attracted)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * attractSpeed;
        }
        else
        {
            // bob up and down slightly
            bobTimer += Time.deltaTime;
            float bobOffset = Mathf.Sin(bobTimer * 3f) * 0.3f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bobOffset);
        }
    }
}