using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodDB
{

    public static void Init()
    {
        foreach (var moo in Moods)
        {
            var moodId = moo.Key;
            var mood = moo.Value;

            mood.Id = moodId;
        }
    }

    public static Dictionary<MoodId, Mood> Moods { get; set; } = new Dictionary<MoodId, Mood>()
    {
        {
            MoodId.great,
            new Mood()
            {
                Name = "Great",
                StartMessage = "Worker is engaged to work",
                OnBeforeShift = (Worker worker) =>
                {
                    worker.IncreaseSpeed(worker.MaxSpeed);
                    worker.StatusChanges.Enqueue($"{worker.Base.Name} will work faster");
                }
            }
        },
        {
            MoodId.happy,
            new Mood()
            {
                Name = "Happy",
                StartMessage = "Worker is happy",
                OnBeforeShift = (Worker worker) =>
                {
                    worker.IncreaseSpeed(worker.MaxSpeed / 3);
                    worker.StatusChanges.Enqueue($"{worker.Base.Name} will work happy");
                }
            }
        },
        {
            MoodId.sad,
            new Mood()
            {
                Name = "Sad",
                StartMessage = "Worker is sad",
                OnBeforeShift = (Worker worker) =>
                {
                    worker.DecreaseSpeed(worker.MaxSpeed / 5);
                    worker.StatusChanges.Enqueue($"{worker.Base.Name} is sad");
                }
            }
        },
        {
            MoodId.angry,
            new Mood()
            {
                Name = "Angry",
                StartMessage = "Worker is angry",
                OnBeforeShift = (Worker worker) =>
                {
                    worker.DecreaseSpeed(worker.MaxSpeed / 2);
                    worker.StatusChanges.Enqueue($"{worker.Base.Name} needs motivation");
                }
            }
        },
        {
            MoodId.quietquit,
            new Mood()
            {
                Name = "Quiet Quitting",
                StartMessage = "Worker won't work",
                OnBeforeShift = (Worker worker) =>
                {
                    worker.DecreaseSpeed(worker.MaxSpeed);
                    worker.StatusChanges.Enqueue($"{worker.Base.Name} is angry at you boss");
                }
            }
        }
    };

   
}

public enum MoodId
{
    great, happy, sad, angry, quietquit
}