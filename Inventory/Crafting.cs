using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    public Inventory inv;

    public void CraftItem(string itemName)
    {
        Recipe r;

        if (inv.MeetsCraftingRequirements(itemName, out r))
        {
            inv.GiveItem(itemName, 1);
            
            for(int i = 0; i < r.items.Count; i++)
            {
                inv.RemoveItem(r.items[i], r.itemCounts[i]);
            }
        }
    }
}
