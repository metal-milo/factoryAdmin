using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsController : MonoBehaviour
{

    List<QuestBase> allQuests = new List<QuestBase>();


    private void Awake()
    {
        Dictionary<string, QuestBase> objects = QuestDB.objects;
        foreach (var objectQuest in objects)
        {
            allQuests.Add(objectQuest.Value);
        }

        //StartCoroutine(ActivateQuest());
    }

    public IEnumerator ActivateQuest()
    {
        Quest activateQuest = new Quest(allQuests[0]);
        yield return activateQuest.StartQuest();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
