using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slotCount = 9;
    private TileType[] items;
    private int[] quantities;
    private int selectedSlot = 0;
    void Start()
    {
        items = new TileType[slotCount];
        quantities = new int[slotCount];

        for(int i = 0; i < slotCount; i++)
        {
            items[i] = TileType.Air;
            quantities[i] = 0;
        }
    }

    public void AddItem(TileType item)
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (items[i] == item)
            {
                quantities[i]++;
                return;
            }
        }

        for (int i = 0; i < slotCount; i++)
        {
            if(items[i] == TileType.Air)
            {
                items[i] = item;
                quantities[i] = 1;
                return;
            }
        }
    }

    public TileType getSelectedItem()
    {
        return items[selectedSlot];
    }

    public bool UseSelectedItem()
    {
        if (items[selectedSlot] == TileType.Air) return false;
        quantities[selectedSlot]--;
        if(quantities[selectedSlot] <= 0)
        {
            items[selectedSlot] = TileType.Air;
        }
        return true;
    }

    public void SelectSlot(int index)
    {
        selectedSlot = Mathf.Clamp(index, 0, slotCount - 1);
    }

    public TileType GetItem(int index) => items[index];
    public int GetQuantity(int index) => quantities[index];
    public int GetSelectedSlot() => selectedSlot;

}
