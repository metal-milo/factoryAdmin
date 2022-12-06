using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new marketing item")]
public class MarketingItem : ItemBase
{
    [SerializeField] bool tv;
    [SerializeField] bool radio;
    [SerializeField] bool social;
    
    public override bool UseOnMarket(Marketplace marketplace)
    {
        if (tv)
        {
            if (marketplace.tv)
            {
                return false;
            }

            marketplace.TvAd(Duration);
        }

        if (radio)
        {
            if (marketplace.radio)
            {
                return false;
            }
            marketplace.RadioAd(Duration);
        }
        
        if (social)
        {
            if (marketplace.socialMedia)
            {
                return false;
            }
            marketplace.SocialMediaAd(Duration);
            
        }
        return true;
    }

    
}



