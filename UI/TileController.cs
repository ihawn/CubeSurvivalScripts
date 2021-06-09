using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    public TileController backTile;
    public Text buttonText;
    public string whatToCraft;
    public bool isRoot;

    private void OnEnable()
    {
        if(!isRoot)
            SetButtonActive(false);
    }

    public void SetButtonActive(bool b)
    {
        if (buttonText != null)
            buttonText.enabled = b;
        if (GetComponent<Button>() != null)
            GetComponent<Button>().enabled = b;
        if (GetComponent<Image>() != null)
            GetComponent<Image>().enabled = b;
    }
}
