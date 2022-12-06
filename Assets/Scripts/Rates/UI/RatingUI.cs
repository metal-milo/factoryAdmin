using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingUI : MonoBehaviour
{
    [SerializeField] Text ratingText;
    [SerializeField] GameObject ratingList;
    [SerializeField] RateSlotUI rateSlotUI;
    [SerializeField] Text Description;

    List<RateSlotUI> slotUIList;

    Rating rating;
    RectTransform rateListRect;

    int selection = 0;
    const int itemsInViewPort = 5;


    private void Awake()
    {
        rating = Rating.GetRatings();
        rateListRect = ratingList.GetComponent<RectTransform>();
    }
    private void Start()
    {
        UpdateRateList();
        Rating.i.OnRatingChange += CalculateRating;
    }

    void UpdateRateList()
    {
        foreach (Transform child in ratingList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<RateSlotUI>();
        foreach (var rateSlot in rating.GetSlots())
        {
            var slotUIObj = Instantiate(rateSlotUI, ratingList.transform);
            slotUIObj.SetData(rateSlot);

            slotUIList.Add(slotUIObj);

        }

        UpdateSelection();

    }

    public void Show()
    {
        gameObject.SetActive(true);
        CalculateRating();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void CalculateRating()
    {
        float posRatings = (float)Rating.i.Positive;
        float negRatings = (float)Rating.i.Negative;
        float totRatings = posRatings / (posRatings + negRatings);
        float ratings = (float)Math.Round(totRatings, 2) * 5;
        float roundRatings = (float)Math.Round(ratings * 2) / 2;
        SetRatingTxt(roundRatings.ToString());
    }

    void SetRatingTxt(string rating)
    {
        ratingText.text = rating;
    }


    public void HandleUpdate(Action onBack)
    {
        int prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.S))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.W))
            --selection;

        selection = Mathf.Clamp(selection, 0, rating.GetSlots().Count - 1);


        if (prevSelection != selection)
        {
            UpdateSelection();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }

    }

    void UpdateSelection()
    {
        var slots = rating.GetSlots();

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selection)
            {
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                slotUIList[i].NameText.color = Color.black;
            }

        }

        selection = Mathf.Clamp(selection, 0, slots.Count - 1);
        if (slots.Count > 0)
        {
            var rate = slots[selection].Rate;
            Description.text = rate.Description;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selection - (itemsInViewPort / 2), 0, selection) * slotUIList[0].Height;
        rateListRect.localPosition = new Vector2(rateListRect.localPosition.x, scrollPos);

    }

}
