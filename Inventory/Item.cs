using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int id;
    public int quantity;
    public bool stackable;
    public int stackSize;
    public string name;
    public string description;
    public Sprite icon;
    public Dictionary<string, int> stats = new Dictionary<string, int>();

    public Item(int id, int quantity, bool stackable, int stackSize, string name, string description, Sprite icon, Dictionary<string, int> stats)
    {
        this.id = id;
        this.quantity = quantity;
        this.stackable = stackable;
        this.stackSize = stackSize;
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.stats = stats;
    }

    public Item(Item item)
    {
        this.id = item.id;
        this.quantity = item.quantity;
        this.stackable = item.stackable;
        this.stackSize = item.stackSize;
        this.name = item.name;
        this.description = item.description;
        this.icon = item.icon;
        this.stats = item.stats;
    }
}
