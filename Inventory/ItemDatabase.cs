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
                new Item(0, true, 64, "Limestone", "A good stone for building", inventoryIcons[0],
                new Dictionary<string, int>
                {
                    {"Strength", 16},
                    {"Durability", 10 }
                }),

                new Item(1, true, 64, "Quartz", "A white crystal. It emits a faint glow", inventoryIcons[1],
                new Dictionary<string, int>
                {
                    {"Magic", 1},
                    {"Value", 10 }
                }),

                new Item(2, true, 64, "ObsidianKey", "A dark colored reflective key. Its color matches that of the walls", inventoryIcons[2],
                new Dictionary<string, int>
                {
                    {"Value", 100 }
                }),

                new Item(3, true, 64, "OakFloor", "A sturdy oaken floor", inventoryIcons[2],
                new Dictionary<string, int>
                {
                    {"Strength", 10 },
                    {"Durability", 10 }
                })
        };
    }
}
