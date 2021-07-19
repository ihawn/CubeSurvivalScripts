using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public Sprite[] inventoryIcons;

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
                new Item(0, 256, false, "Limestone", "A good stone for building", inventoryIcons[0],
                new Dictionary<string, int>
                {
                    {"Strength", 16},
                    {"Durability", 10 }
                }),

                new Item(1, 256, false, "Quartz", "A white crystal. It emits a faint glow", inventoryIcons[1],
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

                new Item(3, 256, false, "OakWood", "A sturdy oaken branch", inventoryIcons[3],
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
                })
        };
    }
}
