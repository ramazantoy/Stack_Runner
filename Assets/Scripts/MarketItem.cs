using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MarketItem : MonoBehaviour
{
    public  int itemId,weardId;
    public int price;
    public Text price_txt;
    public Button buyButton, equipButton, unequipButton;
    public GameObject itemPrefab;
    public bool HasItem()//item satın alınmış mı

    {
        //0 : satın alınmamış
        //1 : satın alınmış giyinmemiş
        //2 : satın alınmış giyinmiş
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;//alınmış ise false var ise true
        return hasItem;
    }
    public bool IsEquipped()//item satın alınmış mı

    {
        //0 : satın alınmamış
        //1 : satın alınmış giyinmemiş
        //2 : satın alınmış giyinmiş
        bool equipItem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;// giyinmiş ise false var ise true
        return equipItem;
    }
    public void InitializeItem()
    {
        price_txt.text = "" + price;
        if (HasItem())
        {
            buyButton.gameObject.SetActive(false);
            if (IsEquipped())
            {
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }
    public void BuyItem()
    {
        if (!HasItem())
        {
            int money = PlayerPrefs.GetInt("money");
            if (money >= price)
            {
                CharacterController.Current.itemAudioSource.PlayOneShot(CharacterController.Current.buyAudioClip, 0.1f);
                LevelController.Current.giveMoney(-price);
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }
    public void EquipItem()
    {
        unEquipItem();
      
        MarketController.Current.equippedItems[weardId]=Instantiate(itemPrefab, CharacterController.Current.wearSpots[weardId].transform).GetComponent<Item>();
        MarketController.Current.equippedItems[weardId].itemId = itemId;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2);

    }
    public void unEquipItem()
    {
        Item equippedItem = MarketController.Current.equippedItems[weardId];
        if (equippedItem != null)
        {
            MarketItem marketItem = MarketController.Current.items[itemId];
            PlayerPrefs.SetInt("item" + marketItem.itemId,1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }
    }
    public void EquipItemButton()
    {
        CharacterController.Current.itemAudioSource.PlayOneShot(CharacterController.Current.equipItemAudioClip,0.1f);
        EquipItem();
    }
    public void UnEquipItemButton()
    {
        CharacterController.Current.itemAudioSource.PlayOneShot(CharacterController.Current.unEquipAudioClip, 0.1f);
        unEquipItem();
    }
}
