using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public Sprite[] inventoryIcons;
    public int generalStackSize = 1024;

    private void Awake()
    {
        BuildDatabase();
    }

    public Item GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    public Item GetItem(string itemName)
    {
        return items.Find(item => item.name == itemName);
    }

    void BuildDatabase()
    {
        items = new List<Item>
        {
                new Item(0, generalStackSize, false, "Limestone", "A good stone for building", inventoryIcons[0],
                new Dictionary<string, int>
                {
                    {"Strength", 16},
                    {"Durability", 10 }
                }),

                new Item(1, generalStackSize, false, "Quartz", "A white crystal. It emits a faint glow", inventoryIcons[1],
                new Dictionary<string, int>
                {
                    {"Magic", 1},
                    {"Value", 10 }
                }),

                new Item(2, 16, false, "ObsidianKey", "A dark colored reflective key. Its color matches that of the walls", inventoryIcons[2],
                new Dictionary<string, int>
                {
                    {"Value", 100 }
                }),

                new Item(3, generalStackSize, false, "OakWood", "A sturdy oaken branch", inventoryIcons[3],
                new Dictionary<string, int>
                {
                    {"Strength", 10 },
                    {"Durability", 10 }
                }),

                new Item(4, 1, true, "IronAxe", "Slightly chipped but gets the job done", inventoryIcons[4],
                new Dictionary<string, int>
                {
                    {"Strength", 10 },
                    {"Durability", 10 },
                    {"DPH", 10}
                }),

                new Item(5, generalStackSize/4, false, "Acorn", "The seed from a meadow oak. Could this grow if left on the ground?", inventoryIcons[5],
                new Dictionary<string, int>
                {
                    {"Value", 10 }
                }),

                new Item(6, generalStackSize/4, false, "BirchSeed", "The seed from a meadow birch. Could this grow if left on the ground?", inventoryIcons[6],
                new Dictionary<string, int>
                {
                    {"Value", 10 }
                }),

                new Item(6, generalStackSize, false, "BirchWood", "A piece of birch wood", inventoryIcons[7],
                new Dictionary<string, int>
                {
                    {"Value", 10 }
                }),

                new Item(7, 1, true, "ShortSword", "A basic sword. It's a little chipped", inventoryIcons[8],
                new Dictionary<string, int>
                {
                    { "Strength", 10 },
                    {"Durability", 10 },
                    {"DPH", 10}
                }),

                new Item(8, generalStackSize/4, true, "Raspberry", "A juicy raspberry", inventoryIcons[9],
                new Dictionary<string, int>
                {
                    { "Value", 100 }
                })
        };
    }
}
