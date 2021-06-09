using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Interractions
{
    public static void Door(bool hasKey, WallDoor door, string[] s, GameObject[] choices, Text messageText, float duration, Inventory inv, GameObject keyhole,  string keyholeMessage)
    {
        if (hasKey)
        {
            ShowChoices(s, choices);

            if (Input.GetKeyDown("1"))
            {
                DisplayMessage(messageText, duration, keyholeMessage);
            }
            else if (Input.GetKeyDown("2"))
            {
                HideChoices(choices);
                door.breakNextFrame = true;
                keyhole.gameObject.tag = "Untagged";
                inv.RemoveItem("ObsidianKey", 1);
            }
        }
        else
        {
            ShowChoices(new string[] { s[0] }, choices);

            if (Input.GetKeyDown("1"))
            {
                DisplayMessage(messageText, duration, door.keyholeMessage);
            }
        }

    }

    public static void DisplayMessage(Text t, float duration, string s)
    {
        t.text = s;
    }

    public static void ShowChoices(string[] s, GameObject[] choices)
    {
        for (int i = 0; i < s.Length; i++)
        {
            choices[i].gameObject.SetActive(true);
            choices[i].GetComponentInChildren<Text>().text = s[i];
        }
    }

    public static void HideChoices(GameObject[] choices)
    {
        for (int i = 0; i < choices.Length; i++)
            choices[i].gameObject.SetActive(false);
    }

    public static void Hide(Text t)
    {
        t.text = "";
    }

    static IEnumerator DelayHide(Text t, float duration)
    {
        yield return new WaitForSeconds(duration);
        t.text = "";
    }
}
