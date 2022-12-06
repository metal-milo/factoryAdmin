using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarItemUI : MonoBehaviour
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

    public void SetData(StarSlot starSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = starSlot.Star.Name;
        priceText.text = $"$ {starSlot.Star.Price}";
    }

    public void SetNameAndPrice(StarBase star)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = star.Name;
        priceText.text = $"$ {star.Price}";
    }
}
