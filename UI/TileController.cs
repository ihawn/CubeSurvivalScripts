using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    public TileController backTile;
    public Transform goPosition;
    public Text buttonText;
    public bool isRoot;

    private void Start()
    {
        if(!isRoot)
            SetButtonActive(false);
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.position = goPosition.position;
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
