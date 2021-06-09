using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{
    public List<Recipe> recipes = new List<Recipe>();

    private void Awake()
    {
        BuildDatabase();
    }

    void BuildDatabase()
    {
        recipes = new List<Recipe>
        {
            new Recipe("ObsidianKey", new List<string>{ "Limestone", "Quartz" }, new List<int>{1, 2})
        };
    }
}
