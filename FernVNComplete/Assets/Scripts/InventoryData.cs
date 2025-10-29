using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Game/InventoryData")]
public class InventoryData : ScriptableObject
{
    public List<ApartmentItemData> ownedItems = new List<ApartmentItemData>();

    public void AddItem(ApartmentItemData item)
    {
        if (!ownedItems.Contains(item))
        {
            ownedItems.Add(item);
            Debug.Log($"Added {item.itemName} to Inventory.");
        }
    }

    public bool HasItem(ApartmentItemData item)
    {
        return ownedItems.Contains(item);
    }

    public void ClearInventory()
    {
        ownedItems.Clear();
    }
}
