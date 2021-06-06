using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> characterItems = new List<Item>();
    public ItemDatabase itemDatabase;
    public int visableInventorySize;
    public string[] visableInventory;
    public int[] visableInventoryQuantity;

    private void Start()
    {
        InitializeInventory();

        GiveItem("Limestone", 70);
    }

    void InitializeInventory()
    {
        visableInventory = new string[visableInventorySize];
        visableInventoryQuantity = new int[visableInventorySize];
    }


    public void GiveItem(string name, int quantity)
    {
        Item itemToAdd = itemDatabase.GetItem(name);
        Item check = CheckForItem(name);
        int emptySlot = GetFirstEmptySlot(visableInventory);

        Debug.Log("Empty slot: " + emptySlot);

        if (emptySlot != -1)
        {
            if (check == null)
            {
                characterItems.Add(itemToAdd);
                visableInventory[emptySlot] = itemToAdd.name;
            }

            else
            {
                check.quantity += quantity;

            }

            int q = quantity;
          
            if(itemToAdd.quantity + quantity > itemToAdd.stackSize)
            {
                q = itemToAdd.stackSize;
                GiveItem(name, quantity - itemToAdd.stackSize);
                visableInventoryQuantity[emptySlot] = itemToAdd.stackSize;
                itemToAdd.quantity = itemToAdd.stackSize;
            }

            else
            {
                visableInventoryQuantity[emptySlot] += quantity;
                itemToAdd.quantity += quantity;
            }

            Debug.Log("Added " + q + " " + itemToAdd.name);
        }
        else
            Debug.Log("Item not added. Inventory full");
    }

    public Item CheckForItem(int id)
    {
        return characterItems.Find(item => item.id == id);
    }

    public Item CheckForItem(string name)
    {
        return characterItems.Find(item => item.name == name);
    }

    public void RemoveItem(string name, int quantity)
    {
        Item item = CheckForItem(name);
        if (item != null)
        {
            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                item.quantity = 0;
                characterItems.Remove(item);
            }

            Debug.Log("Item removed: " + item.name);
        }
    }

    int GetFirstEmptySlot(string[] vi)
    {
        for (int i = 0; i < vi.Length; i++)
            if (vi[i] == null)
                return i;

        return -1;
    }
}
