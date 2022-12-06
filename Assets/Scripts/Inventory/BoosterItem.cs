using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new booster item")]
public class BoosterItem : ItemBase
{
    [SerializeField] int speed;

    [SerializeField] int salary;

    [SerializeField] MoodId mood;

    [SerializeField] bool giveRaise;

    [SerializeField] bool canReuse = false;
    public override bool UseOnWorker(Worker worker)
    {
        // Increase Speed
        worker.IncreaseSpeed(speed);

        // Recover Status
        if (mood != MoodId.happy)
        {
            if (worker.Status == null)
                return false;

            if (giveRaise)
            {
                canReuse = true;
                worker.GiveRaise(salary);

            }
            else
            {
                if (worker.Status.Id == mood)
                    worker.Motivate();
                else
                    return false;
            }
        }

        return true;
    }

    public override bool IsReusable => canReuse;
}
