using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingMenu : MonoBehaviour
{
    public TileController activeTile;
    public List<TileController> branchedTiles;
    public RectTransform activeTileTransform;
    public List<Vector2> branchedTilePositions;
    public float offset, lerpSpeed, spacing;
    Crafting craft;


    void Start()
    {
        craft = GetComponent<Crafting>();
        activeTile.transform.localScale = Vector3.one;
    }


    void Update()
    {
        UpdateTilePositions();
    }

    public void ClickedMenuTile(TileController tileController)
    {
        if(tileController.whatToCraft != "")
        {
            craft.CraftItem(tileController.whatToCraft);
        }
        else
        {
            TraverseMenuTree(tileController);
        }
    }

    void Craft()
    {

    }

    void TraverseMenuTree(TileController tileController)
    {
        if (activeTile == tileController)
        {
            if (tileController.backTile != null)
            {
                activeTile = tileController.backTile;
                tileController.backTile.SetButtonActive(true);
            }
        }
        else
        {
            activeTile = tileController;
            if(tileController.backTile != null)
                tileController.backTile.SetButtonActive(false);
        }

        for (int i = 0; i < branchedTiles.Count; i++)
        {
            branchedTiles[i].SetButtonActive(false);
        }

        tileController.SetButtonActive(true);
        tileController.transform.position = activeTileTransform.position;

        branchedTiles.Clear();
        branchedTilePositions.Clear();

        foreach (Transform child in activeTile.transform)
        {
            if (child.GetComponent<TileController>() != null)
                branchedTiles.Add(child.GetComponent<TileController>());
        }

        int length = branchedTiles.Count;

        for (int i = 0; i < length; i++)
        {
            branchedTiles[i].SetButtonActive(true);
            branchedTiles[i].transform.position = activeTileTransform.position;
            float inside = -Mathf.PI / 2 - (length - 1) * Mathf.PI / (spacing * 2f) + i * Mathf.PI / (spacing);
            branchedTilePositions.Add(offset * new Vector2(Mathf.Cos(inside), Mathf.Sin(inside)) + new Vector2(activeTileTransform.position.x, activeTileTransform.position.y));
        }
    }

    void UpdateTilePositions()
    {
        if (activeTile != null)
        {
            for (int i = 0; i < branchedTiles.Count; i++)
            {
                branchedTiles[i].transform.position = Vector3.Lerp(branchedTiles[i].transform.position, branchedTilePositions[i], lerpSpeed*Time.deltaTime);
                branchedTiles[i].transform.localScale = Vector3.Lerp(branchedTiles[i].transform.localScale, Vector3.one*0.5f, lerpSpeed * Time.deltaTime);
            }

            activeTile.transform.position = Vector3.Lerp(activeTile.transform.position, activeTileTransform.position, lerpSpeed * Time.deltaTime);
            activeTile.transform.localScale = Vector3.Lerp(activeTile.transform.localScale, Vector3.one, lerpSpeed * Time.deltaTime);
        }
    }
}
