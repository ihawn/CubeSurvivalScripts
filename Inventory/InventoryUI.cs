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
    public int selectedSlot;

    private void Start()
    {
        SetSelectedSlot(0);
    }

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

    public void ChangeSelectedSlot(float scrollDelta)
    {
        if (scrollDelta < 0)
            SetSelectedSlot(selectedSlot + 1);
        else
            SetSelectedSlot(selectedSlot - 1);
    }

    void SetSelectedSlot(int i)
    {
        print(i);
        visableInventorySlots[selectedSlot].color = notSelectedColor;

        if (i < visableInventorySlots.Length && i >= 0)
            selectedSlot = i;
        else if (i < 0)
            selectedSlot = visableInventorySlots.Length - 1;
        else if (i >= visableInventorySlots.Length)
            selectedSlot = 0;

        visableInventorySlots[selectedSlot].color = selectedColor;
    }
}
