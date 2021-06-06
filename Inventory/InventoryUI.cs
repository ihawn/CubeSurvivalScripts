using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory;
    public Color selectedColor, notSelectedColor;
    public Image[] visableInventorySlots;
    public Image[] inventorySprites;
    public Text[] inventoryQuantity;


    private void Update()
    {
        UpdateInventoryUI();
    }


    void UpdateInventoryUI()
    {
        for (int i = 0; i < playerInventory.visableInventorySize; i++)
        {
            if(playerInventory.visableInventory[i] != null)
            {
                inventorySprites[i].sprite = playerInventory.CheckForItem(playerInventory.visableInventory[i]).icon;
                inventorySprites[i].color = new Vector4(1f, 1f, 1f, 1f);
                inventoryQuantity[i].text = playerInventory.visableInventoryQuantity[i].ToString();
            }

            else
            {
                inventorySprites[i].sprite = null;
                inventorySprites[i].color = new Vector4(1f, 1f, 1f, 0f);
                inventoryQuantity[i].text = "";
            }

        }
    }
}
