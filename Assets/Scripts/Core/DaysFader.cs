using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DaysFader : MonoBehaviour
{
    public static DaysFader i { get; private set; }
    [SerializeField] Image image;
    [SerializeField] Text daysText;
    private void Awake()
    {
        i = this;
        
    }

    

    public IEnumerator FadeIn(float time, string message)
    {
        daysText.text = message;
        daysText.gameObject.SetActive(true);
        yield return daysText.DOFade(1f, time);
        yield return image.DOFade(1f, time).WaitForCompletion();
        
        
    }

    public IEnumerator FadeOut(float time)
    {
        yield return daysText.DOFade(0f, time);
        yield return image.DOFade(0f, time).WaitForCompletion();
        //daysText.gameObject.SetActive(false);
    }

    public IEnumerator GameOver(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
        daysText.text = "Game Over";
        daysText.gameObject.SetActive(true);

    }

}



