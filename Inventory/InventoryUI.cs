using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Inventory playerInventory;
    public Color selectedColor, notSelectedColor;
    public Image[] visableInventorySlots;
    public Image[] inventorySprites;
    public Text[] inventoryQuantity;
    public int selectedSlot;

    public Transform rightHand;
    public GameObject rightHandObject;
    GameObject currentHover;

    private void Start()
    {
        SetSelectedSlot(0);
    }

    private void Update()
    {
        UpdateInventoryUI();
        UpdateSlotPosition();
        print(currentHover);
        print(visableInventorySlots[0].gameObject);
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
        visableInventorySlots[selectedSlot].color = notSelectedColor;

        if (i < visableInventorySlots.Length && i >= 0)
            selectedSlot = i;
        else if (i < 0)
            selectedSlot = visableInventorySlots.Length - 1;
        else if (i >= visableInventorySlots.Length)
            selectedSlot = 0;

        visableInventorySlots[selectedSlot].color = selectedColor;
        UpdateItemInHand();
    }

    void UpdateItemInHand()
    {
        GameObject retrievedByName = playerInventory.RetrieveStaticPrefabByName(playerInventory.visableInventory[selectedSlot]);
        Destroy(rightHandObject);

        if (playerInventory.visableInventory[selectedSlot] != "" && retrievedByName != null)
            rightHandObject = Instantiate(retrievedByName, rightHand.transform.position, rightHand.transform.rotation);
        
        if(rightHandObject != null)
            rightHandObject.transform.parent = rightHand;
    }

    void UpdateSlotPosition()
    {
        bool dragging = false;
        
        if (Input.GetMouseButton(0) && currentHover != null && ImagesContainObject(visableInventorySlots, currentHover))
            dragging = true;
        else if (Input.GetMouseButtonUp(0))
            dragging = false;

        if(dragging)
        {

        }
        print(dragging);
    }

    bool ImagesContainObject(Image[] a, GameObject g)
    {
        for(int i = 0; i < a.Length; i++)
        {
            if (a[i].gameObject == g)
                return true;
        }
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentHover = eventData.pointerCurrentRaycast.gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentHover = null;
    }
}
