using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
    public string whatToMake;
    public List<string> items;
    public List<int> itemCounts;

    public Recipe(string whatToMake, List<string> items, List<int> itemCounts)
    {
        this.whatToMake = whatToMake;
        this.items = items;
        this.itemCounts = itemCounts;
    }

    public Recipe(Recipe recipe)
    {
        this.whatToMake = recipe.whatToMake;
        this.items = recipe.items;
        this.itemCounts = recipe.itemCounts;
    }
}
