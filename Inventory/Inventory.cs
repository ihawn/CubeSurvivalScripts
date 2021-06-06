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
        int emptySlot = GetFirstEmptySlot(visableInventory);
        int slotwithItem = GetSlotWithName(name);
        int q = quantity;

        //inventory is full (slotwise)
        if(emptySlot == -1)
        {
            //check if inventory contains item and if so if maximum stack size is reached
        }

        //inventory is not full
        else
        {
            int slotWithRoom = SlotWithRoom(itemToAdd);

            //If theres a slot with the item added but it still has room
            if (slotWithRoom != -1)
            {
                //check if items added exceeds stacksize
                if (quantity + visableInventoryQuantity[slotWithRoom] > itemToAdd.stackSize)
                {
                    GiveItem(name, quantity + visableInventoryQuantity[slotWithRoom] - itemToAdd.stackSize); //Give remainder of quantity to next stack
                    visableInventoryQuantity[slotWithRoom] = itemToAdd.stackSize;
                }

                //if it fits
                else
                {
                    visableInventoryQuantity[slotWithRoom] += quantity;
                }
            }

            //No slot with this item has room or this item isn't in the inventory at all 
            else
            {
                visableInventory[emptySlot] = name; //put name of item into slot

                //check if items added exceeds stacksize
                if (quantity > itemToAdd.stackSize)
                {
                    visableInventoryQuantity[emptySlot] = itemToAdd.stackSize;
                    GiveItem(name, quantity - itemToAdd.stackSize); //Give remainder of quantity to next stack
                }

                //If we can fit the item into the current slot
                else
                {
                    visableInventoryQuantity[emptySlot] = quantity;
                }
            }

        }

       /* Debug.Log("Empty slot: " + emptySlot);

        if (emptySlot != -1)
        {
            if (check == null)
            {
                characterItems.Add(itemToAdd);
                visableInventory[emptySlot] = itemToAdd.name;

                if (quantity > itemToAdd.stackSize)
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
            }

            else
            {
                if (visableInventoryQuantity[GetSlotWithName(name)] + quantity > itemToAdd.stackSize)
                {
                    check.quantity = itemToAdd.stackSize;
                    characterItems.Add(itemToAdd);
                    visableInventoryQuantity[emptySlot] = itemToAdd.stackSize;
                    visableInventory[emptySlot] = itemToAdd.name;
                }

                else
                {
                    visableInventoryQuantity[GetSlotWithName(name)] += quantity;

                    check.quantity += quantity;
                }
            }

            Debug.Log("Added " + q + " " + itemToAdd.name);
        }
        else
            Debug.Log("Item not added. Inventory full");*/
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

            characterItems.Remove(item);

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

    int GetSlotWithName(string name)
    {
        for (int i = 0; i < visableInventory.Length; i++)
            if (visableInventory[i] == name)
                return i;

        return -1;
    }

    int SlotWithRoom(Item item)
    {
        for (int i = 0; i < visableInventoryQuantity.Length; i++)
            if (visableInventoryQuantity[i] < item.stackSize && visableInventory[i] == item.name)
                return i;

        return -1;
    }
}
