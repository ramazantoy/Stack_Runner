using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current;
    public List<MarketItem> items;
    public List<Item> equippedItems;
    public GameObject marketMenu;
    public void initalizeMarketController()
    {
        Current = this;
        foreach(MarketItem tmp in items)
        {
            tmp.InitializeItem();
        }
    }
    public  void ActiveMarketMenu(bool active)
    {
        marketMenu.SetActive(active);
    }
}
