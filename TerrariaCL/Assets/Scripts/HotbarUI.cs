using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public int slotCount = 9;

    private GameObject[] slots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slots = new GameObject[slotCount];
        for(int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, transform);
            slot.name = $"Slot{i}";
            slots[i] = slot;
        }
    }
}
