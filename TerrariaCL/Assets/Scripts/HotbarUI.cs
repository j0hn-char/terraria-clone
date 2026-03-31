using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    public Sprite defaultSlotSprite;
    public GameObject slotPrefab;
    public int slotCount = 9;
    public Sprite dirtSprite;
    public Sprite stoneSprite;
    public Sprite ironSprite;
    public Sprite bronzeSprite;
    public Sprite titaniumSprite;
    public Sprite selectedSlotSprite;

    private GameObject[] slots;
    private Image[] slotImages;
    private Image[] itemImages;
    private TextMeshProUGUI[] quantityTexts;
    private Inventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        slots = new GameObject[slotCount];
        slotImages = new Image[slotCount];
        itemImages = new Image[slotCount];
        quantityTexts = new TextMeshProUGUI[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            slot.name = $"Slot{i}";
            slots[i] = slot;
            slotImages[i] = slot.GetComponent<Image>();

            // item icon
            GameObject itemIcon = new GameObject("ItemIcon");
            itemIcon.transform.SetParent(slot.transform);
            Image itemImage = itemIcon.AddComponent<Image>();
            itemImage.rectTransform.sizeDelta = new Vector2(32, 32);
            itemImage.rectTransform.anchoredPosition = Vector2.zero;
            itemImage.enabled = false;
            itemImages[i] = itemImage;

            // quantity text
            GameObject textObj = new GameObject("Quantity");
            textObj.transform.SetParent(slot.transform);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.fontSize = 12;
            text.alignment = TextAlignmentOptions.BottomRight;
            text.rectTransform.sizeDelta = new Vector2(40, 40);
            text.rectTransform.anchoredPosition = Vector2.zero;
            quantityTexts[i] = text;
        }
    }

    void Update()
    {
        // slot selection with number keys
        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                inventory.SelectSlot(i);
        }

        // scroll wheel selection
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll > 0) inventory.SelectSlot(inventory.GetSelectedSlot() - 1);
        if (scroll < 0) inventory.SelectSlot(inventory.GetSelectedSlot() + 1);

        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slotCount; i++)
        {
            // highlight selected slot
            slotImages[i].sprite = i == inventory.GetSelectedSlot() ? selectedSlotSprite : defaultSlotSprite;

            // show item icon
            TileType item = inventory.GetItem(i);
            if (item != TileType.Air)
            {
                itemImages[i].enabled = true;
                itemImages[i].sprite = GetSpriteForTile(item);
                quantityTexts[i].text = inventory.GetQuantity(i).ToString();
            }
            else
            {
                itemImages[i].enabled = false;
                quantityTexts[i].text = "";
            }
        }
    }

    Sprite GetSpriteForTile(TileType type)
    {
        switch (type)
        {
            case TileType.Dirt: return dirtSprite;
            case TileType.Stone: return stoneSprite;
            case TileType.Iron: return ironSprite;
            case TileType.Bronze: return bronzeSprite;
            case TileType.Titanium: return titaniumSprite;
            default: return null;
        }
    }
}