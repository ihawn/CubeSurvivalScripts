using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    Inventory inv;

    void Start()
    {
        inv = GetComponent<Inventory>();
    }

    public void CraftItem(string itemName)
    {
        if (inv.MeetsCraftingRequirements(itemName))
        {
            inv.GiveItem(itemName, 1);
        }
    }
}
