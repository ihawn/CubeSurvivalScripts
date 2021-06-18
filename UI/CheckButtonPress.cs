using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckButtonPress : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryUI invUI;
    public bool buttonHovered;
    public bool buttonPressed;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        buttonHovered = true;
        invUI.hoveredSlot = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonHovered = false;
        invUI.hoveredSlot = null;
    }
}
