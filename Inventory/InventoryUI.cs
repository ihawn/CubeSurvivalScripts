using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    public Inventory playerInventory;
    public Color selectedColor, notSelectedColor;
    public Image[] visableInventorySlots;
    public Image[] inventorySprites;
    public Text[] inventoryQuantity;
    public int selectedSlot;

    public Transform rightHand;
    public GameObject rightHandObject;
    public GameObject hoveredSlot;
    public GameObject pressedSlot;

    Vector3 dragOffset;

    public bool buttonPressed;
    public float buttonDragTolerance = 20f;

    private void Start()
    {
        SetSelectedSlot(0);
    }

    private void Update()
    {
        UpdateSlotPosition();
    }

    private void LateUpdate()
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
        {
            rightHandObject = Instantiate(retrievedByName, rightHand.transform.position, rightHand.transform.rotation);
        }
        
        if(rightHandObject != null)
            rightHandObject.transform.parent = rightHand;
        
    }

    void UpdateSlotPosition()
    {

        bool wasNull = false;

        if (pressedSlot == null)
            wasNull = true;

        if(Input.GetMouseButtonDown(0) && hoveredSlot != null)
        {
            pressedSlot = hoveredSlot;
            hoveredSlot = null;
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(pressedSlot != null)
            {
                GameObject newParent = ShortestDistance(pressedSlot.GetComponent<SlotController>().sprite, visableInventorySlots);              
                pressedSlot.GetComponent<SlotController>().sprite.transform.position = newParent.transform.position;
                pressedSlot.GetComponent<SlotController>().sprite.transform.SetParent(newParent.transform);
                newParent.GetComponent<SlotController>().sprite.transform.position = pressedSlot.transform.position;
                newParent.GetComponent<SlotController>().sprite.transform.SetParent(pressedSlot.transform);
                GameObject temp = pressedSlot.GetComponent<SlotController>().sprite;
                pressedSlot.GetComponent<SlotController>().sprite = newParent.GetComponent<SlotController>().sprite;
                newParent.GetComponent<SlotController>().sprite = temp;
                Image temp1_5 = inventorySprites[pressedSlot.GetComponent<SlotController>().slotID];
                inventorySprites[pressedSlot.GetComponent<SlotController>().slotID] = inventorySprites[newParent.GetComponent<SlotController>().slotID];
                inventorySprites[newParent.GetComponent<SlotController>().slotID] = temp1_5;
                Text temp1_9 = inventoryQuantity[pressedSlot.GetComponent<SlotController>().slotID];
                inventoryQuantity[pressedSlot.GetComponent<SlotController>().slotID] = inventoryQuantity[newParent.GetComponent<SlotController>().slotID];
                inventoryQuantity[newParent.GetComponent<SlotController>().slotID] = temp1_9;
                string temp2 = playerInventory.visableInventory[pressedSlot.GetComponent<SlotController>().slotID];
                playerInventory.visableInventory[pressedSlot.GetComponent<SlotController>().slotID] = playerInventory.visableInventory[newParent.GetComponent<SlotController>().slotID];
                playerInventory.visableInventory[newParent.GetComponent<SlotController>().slotID] = temp2;
                int temp3 = playerInventory.visableInventoryQuantity[pressedSlot.GetComponent<SlotController>().slotID];
                playerInventory.visableInventoryQuantity[pressedSlot.GetComponent<SlotController>().slotID] = playerInventory.visableInventoryQuantity[newParent.GetComponent<SlotController>().slotID];
                playerInventory.visableInventoryQuantity[newParent.GetComponent<SlotController>().slotID] = temp3;
                newParent.GetComponent<SlotController>().sprite.transform.SetSiblingIndex(0);
            }

            pressedSlot = null;
        }

        if(pressedSlot != null)
        {
            if(wasNull)
                dragOffset = pressedSlot.transform.position - Input.mousePosition;

            Vector3 pos = Input.mousePosition + dragOffset;
            pressedSlot.GetComponent<SlotController>().sprite.transform.position = new Vector3(pressedSlot.GetComponent<SlotController>().sprite.transform.position.x, pos.y, pressedSlot.GetComponent<SlotController>().sprite.transform.position.z);
        }
    }

    GameObject ShortestDistance(GameObject g1, Image[] g2)
    {
        float minDist = Vector3.Distance(g1.transform.position, g2[0].gameObject.transform.position);
        GameObject minObj = g2[0].gameObject;

        for(int i = 1; i < g2.Length; i++)
        {
            float dist = Vector3.Distance(g1.transform.position, g2[i].gameObject.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                minObj = g2[i].gameObject;
            }
        }

        return minObj;
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

}
