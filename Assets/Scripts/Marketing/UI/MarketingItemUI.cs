using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketingItemUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text priceText;

    RectTransform rectTransform;
    private void Awake()
    {

    }

    public Text NameText => nameText;
    public Text PriceText => priceText;

    public float Height => rectTransform.rect.height;
   
    
    public void SetData(MarketItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Advertisement.Name;
        priceText.text = $"$ {itemSlot.Advertisement.Price}";
    }

    public void SetNameAndPrice(ItemBase advertisement)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = advertisement.Name;
        priceText.text = $"$ {advertisement.Price}";
    }
}


