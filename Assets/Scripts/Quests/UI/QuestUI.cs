using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] GameObject questList;
    [SerializeField] QuestSlotUI questSlotUI;

    List<QuestSlotUI> questUIList;

    QuestList questsList;
    RectTransform questListRect;

    private void Awake()
    {
        questsList = QuestList.GetQuestList();
        questListRect = questList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateQuestList();
        questsList.OnUpdated += UpdateQuestList;
    }

    void UpdateQuestList()
    {
        foreach (Transform child in questList.transform)
        {
            Destroy(child.gameObject);
        }

        questUIList = new List<QuestSlotUI>();
        foreach (var questSlot in questsList.GetActiveQuests())
        {
            var slotUIObj = Instantiate(questSlotUI, questList.transform);
            slotUIObj.SetData(questSlot);

            questUIList.Add(slotUIObj);

        }


    }
}
