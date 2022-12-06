using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Image icon;

    RectTransform rectTransform;



    private void Awake()
    {

    }

    public Text NameText => nameText;
    public Image Icon => icon;

    public float Height => rectTransform.rect.height;

    public void SetData(RateSlot rateSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = rateSlot.Rate.Name;
        icon.sprite = rateSlot.Rate.Icon;
    }

    public void SetNameAndIcon(RateBase rate)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = rate.Name;
        icon.sprite = rate.Icon;
    }
}
