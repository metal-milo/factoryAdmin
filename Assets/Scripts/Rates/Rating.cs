using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rating : MonoBehaviour
{
    [SerializeField] int positive;
    [SerializeField] int negative;
    [SerializeField] List<RateSlot> slots;
    int total;


    
    List<RateBase> positives = new List<RateBase>();
    List<RateBase> negatives = new List<RateBase>();

    public int Positive => positive;
    public int Negative => negative;

    public event Action OnRatingChange;

    public static Rating i { get; private set; }
    private void Awake()
    {
        Dictionary<string, RateBase> objects = RateDB.objects;
        foreach (var objectRate in objects)
        {
            if (objectRate.Value.IsPositive)
            {
                positives.Add(objectRate.Value);
            }
            else
            {
                negatives.Add(objectRate.Value);
            }
        }
        i = this;
    }

    public List<RateSlot> GetSlots()
    {
        return slots;
    }

    public static Rating GetRatings()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Rating>();
    }

    public void AddPositive()
    {
        positive++;
        RateBase newRate = positives[RandomNumber(0, positives.Count)];
        AddRate(newRate);
    }

    public void AddNegative()
    {
        negative++;
        RateBase newRate = negatives[RandomNumber(0, negatives.Count)];
        AddRate(newRate);
    }

    public void AddDefective()
    {
        negative++;
        RateBase newRate = RateDB.GetObjectByName("Defective");
        AddRate(newRate);
    }
    public void AddRate(RateBase rate)
    {
        var currentSlots = GetSlots();

        var rateSlot = currentSlots.LastOrDefault(slot => slot.Rate == rate);
        currentSlots.Insert(0,new RateSlot()
        {
            Rate = rate
        });

        if (currentSlots.Count >= 7)
        {
            currentSlots.RemoveAt(currentSlots.Count - 1);
        }


        OnRatingChange?.Invoke();
    }

    public float GetRating()
    {
        total = positive + negative;
        float posRatings = (float)positive;
        float negRatings = (float)negative;
        float totRatings = posRatings / (posRatings + negRatings);
        float ratings = (float)Math.Round(totRatings, 2) * 5;
        float roundRatings = (float)Math.Round(ratings * 2) / 2;
        return roundRatings;
        
    }

    private int RandomNumber(int minLimit, int maxLimit)
    {
        return UnityEngine.Random.Range(minLimit, maxLimit);
    }

}


[Serializable]
public class RateSlot
{
    [SerializeField] RateBase rate;

    public RateBase Rate
    {
        get => rate;
        set => rate = value;
    }
}
