using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUnit : MonoBehaviour
{
    //[SerializeField] WorkerBase _base;
    //[SerializeField] int level;

    Image image;
    Vector3 orginalPos;

    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = image.transform.localPosition;
    }

    public Worker Worker { get; set; }

    public void Setup(Worker worker)
    {
        Worker = worker;
        GetComponent<Image>().sprite = Worker.Base.FrontSprite;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        image.transform.localPosition = new Vector3(-508f, orginalPos.y);
        image.transform.DOLocalMoveX(orginalPos.x, 1f);
    }

    public IEnumerator PlayGoWorkAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveX(508f, 1f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayLeaveAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveX(-508f, 1f));
        yield return sequence.WaitForCompletion();
    }
}
