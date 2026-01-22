using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;
    
    private ItemDictionary _itemDictionary;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemChoiceBar;
    public GameObject slotPrefab;
    public int slotCount;
    private int _selectedSlot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }
        
        Debug.Log("Inventory is full!");
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.itemID, slotIndex = slotTransform.GetSiblingIndex()});
            }
        }
        return invData;
    }
    
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = _itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
    
    public void SetSelectedSlot(int slotIndex)
    {
        if (inventoryPanel.GetComponentsInChildren<Slot>()[slotIndex].currentItem == null) return;
        _selectedSlot = slotIndex;
        itemChoiceBar.SetActive(true);
    }

    public void UseItem(int characterChoice)
    {
        GameObject selectedItem = inventoryPanel.GetComponentsInChildren<Slot>()[_selectedSlot].currentItem;
        selectedItem.GetComponent<Item>().UseItem(characterChoice);
        itemChoiceBar.SetActive(false);
        PlayerPage.Instance.UpdateStats();
    }
}
