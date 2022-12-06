using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;

    RectTransform rectTransform;

    private void Awake()
    {

    }

    public Text NameText => nameText;

    public float Height => rectTransform.rect.height;

    public void SetData(Quest quest)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = quest.Base.Description;
    }

    public void SetName(QuestBase quest)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = quest.Description;
    }

}
