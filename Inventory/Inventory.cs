using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> characterItems = new List<Item>();
    public RecipeDatabase recipeDatabase;
    public ItemDatabase itemDatabase;
    public int visableInventorySize;
    public string[] visableInventory;
    public int[] visableInventoryQuantity;

    private void Start()
    {
        InitializeInventory();
        GiveItem("Limestone", 50);
        GiveItem("Quartz", 100);
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
        characterItems.Add(itemToAdd);
        int q = quantity;

        //inventory is full (slotwise)
        if(emptySlot == -1)
        {
            //check if inventory contains item and if so if maximum stack size is reached
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

            else
            {
                //Do nothing. Inventory is completely full
            }

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

    }

    public Item CheckForItem(string name)
    {
        return characterItems.Find(item => item.name == name);
    }

    public Recipe GetRecipe(string name)
    {
        return recipeDatabase.recipes.Find(recipe => recipe.whatToMake == name);
    }

    public Item GetItem(string name)
    {
        return itemDatabase.items.Find(item => item.name == name);
    }

    public void RemoveItem(string name, int quantity)
    {
        Item item = CheckForItem(name);
        int slot = GetSlotWithName(name);
        if (slot != -1)
        {
            visableInventoryQuantity[slot] -= quantity;
            if (visableInventoryQuantity[slot] <= 0)
                visableInventory[slot] = null;
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

    public bool HasNumberOfItem(string itemName, int num)
    {
        int sum = 0;

        for(int i = 0; i < visableInventory.Length; i++)
        {
            if (visableInventory[i] == itemName)
                sum += visableInventoryQuantity[i];
            if (sum >= num)
                return true;
        }

        return false;
    }

    public bool MeetsCraftingRequirements(string itemName, out Recipe r)
    {
        r = GetRecipe(itemName);
        int len = r.items.Count;

        for(int i = 0; i < len; i++)
        {
            if (!HasNumberOfItem(r.items[i], r.itemCounts[i]))
                return false;
        }

        return true;
    }
}
