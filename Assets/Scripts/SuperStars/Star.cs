using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stars/Create new star")]
public class Star : StarBase
{
    [SerializeField] bool sales;
    [SerializeField] bool prod;
    [SerializeField] bool quality;
    [SerializeField] bool isMarket;

    public override bool CallingStarMarket(Marketplace marketplace)
    {
        if (sales)
        {
            if (marketplace.salesStar)
            {
                return false;
            }

            marketplace.HireSalesStar(Duration);
        }

        if (quality)
        {
            if (marketplace.qualityStar)
            {
                return false;
            }
            marketplace.HireQualityStar(Duration);
        }
        return true;
    }

   
    public override bool CallingStarProd(Produce production)
    {
        if (prod)
        {
            if (production.prodIncrease)
            {
                return false;
            }
            production.Increase(Duration);
        }
        return true;
    }

    public override bool IsMarket => isMarket;
}
