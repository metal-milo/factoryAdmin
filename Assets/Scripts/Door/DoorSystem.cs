using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EntranceState { Start, PlayerAction, PlayerMove, Busy, PartyScreen, StartWorking}

public class DoorSystem : MonoBehaviour
{
    [SerializeField] WorkerUnit workerUnit;
    [SerializeField] WorkerHud workerHud;
    [SerializeField] DoorDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Text prodText;
    [SerializeField] Text defText;

    private float totalProd = 0f;
    private float totalDefectives = 0;

    public event Action EntranceOver;

    EntranceState state;
    int currentAction;
    int workerIndex = 0;
    bool maxWorkers = false;
    

    WorkerParty workerParty;
    List<Worker> outsideWorkers = new List<Worker>();
    Worker outsideWorker;


    private void Awake()
    {
        workerHud.activateTexts();
        prodText.text = "Expected Products: " + totalProd + " Today";
        defText.text = "Defectives: " + totalDefectives + "%";
    }

    public void SelectStaff(WorkerParty workerParty, List<Worker> outsideWorkers)
    {
        totalDefectives = 0;
        totalProd = 0;
        maxWorkers = false;
        workerIndex = 0;
        this.workerParty = workerParty;
        this.outsideWorkers = outsideWorkers;
        outsideWorker = outsideWorkers[workerIndex];
        StartCoroutine(SetupEntrance());
    }

    public IEnumerator SetupEntrance()
    {
        outsideWorker.OnBeforeShift();
        workerUnit.Setup(outsideWorker);
        workerHud.SetData(workerUnit.Worker);
        dialogBox.SetFaultNames(workerUnit.Worker.Faults);
        

        yield return dialogBox.TypeDialog($"Today {workerUnit.Worker.Base.Name}.");
        yield return new WaitForSeconds(2f);
        yield return ShowStatusChanges(outsideWorker);
        yield return new WaitForSeconds(2.5f);
        partyScreen.Init();

        PlayerAction();
        yield return new WaitForSeconds(2.5f);
        PlayerMove();
    }

    void PlayerAction()
    {
        dialogBox.EnableDialogText(true);
        dialogBox.EnableFaultSelector(false);
        state = EntranceState.PlayerAction;

        if (!maxWorkers)
        {
            StartCoroutine(dialogBox.TypeDialog("Select Enter to work, Leave to view next worker"));
        }
        else
        {
            StartCoroutine(dialogBox.TypeDialog("Select Start to work"));
        }


        
        dialogBox.EnableActionSelector(true);

    }

    public void ValidateWorkers()
    {
        if (workerParty.Workers.Count == 6)
        {
            //StartCoroutine(dialogBox.TypeDialog("Max workers reached."));
            Debug.Log("Max workers");
            maxWorkers = true;
        }
        workerIndex++;
        
        if (workerIndex == (outsideWorkers.Count))
        {
            workerIndex = 0;
        }
        outsideWorker = outsideWorkers[workerIndex];
        if (!maxWorkers)
        {
            if (workerParty.Workers.Contains(outsideWorker))
            {
                ValidateWorkers();
            }
        }
    }

    void PlayerMove()
    {
        //state = EntranceState.PlayerMove;
        //dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableFaultSelector(true);
    }

    public void HandleUpdate()
    {
        if (state == EntranceState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == EntranceState.PartyScreen)
        {
            ShowPartyScreen();
        }

    }

    void OpenPartyScreen()
    {
        Debug.Log("Party Screen");
        state = EntranceState.PartyScreen;
        partyScreen.SetPartyData(workerParty.Workers);
        partyScreen.gameObject.SetActive(true);
    }

    

    public void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.D))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.A))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.S))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.W))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        if (currentAction == 0)
        {

        }
        
        prodText.text = "Expected Products: " + totalProd + " Today";
        defText.text = "Defectives: " + totalDefectives + "%";

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Enter to work, add to party, add products and defectives
                totalProd += outsideWorker.hourlyProduction * 8;
                totalDefectives += outsideWorker.defectivesPercent;
                Debug.Log("Enter");
                ToWork();

                //workerParty.AddWorker
            }
            else if (currentAction == 1)
            {
                //Leave work, do not add to party
                Debug.Log("Leave");
                NextWorker(false);
                //Debug.Log("Leave");
            }
            else if (currentAction == 2)
            {
                // Party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Start
                if (workerParty.Workers.Count != 0)
                {
                    EntranceOver();
                }
                else
                {
                    PlayerAction();
                    PlayerMove();
                }
            }
        }
    }

    public Worker GetNextWorker(int workerIndex)
    {

        return outsideWorkers[workerIndex];
    }

    public void ToWork()
    {
        
        workerParty.AddWorker(outsideWorker);
        NextWorker(true);
    }

    public void NextWorker(bool willWork)
    {
        
        ValidateWorkers();
        //var nextOutsideWorker = FindObjectOfType<humanR>().GetComponent<humanR>().GetRandomWorker(1);
        //var outsideWorkerCopy = new Worker(nextOutsideWorker.Base, nextOutsideWorker.Level);
        //SelectStaff(workerParty, outsideWorkerCopy);
        StartCoroutine(NextWorkerRoutine(GetNextWorker(workerIndex), willWork));
        
    }
   

    public IEnumerator NextWorkerRoutine(Worker nextOutsideWorker, bool willWork)
    {
        if (willWork)
            yield return workerUnit.PlayGoWorkAnimation();
        else
            yield return workerUnit.PlayLeaveAnimation();
        nextOutsideWorker.OnBeforeShift();
        workerUnit.Setup(nextOutsideWorker);
        workerHud.SetData(workerUnit.Worker);
        dialogBox.EnableFaultSelector(false);
        dialogBox.SetFaultNames(workerUnit.Worker.Faults);
        

        if (!maxWorkers)
        {
            dialogBox.EnableDialogText(true);
            yield return dialogBox.TypeDialog($"Today {workerUnit.Worker.Base.Name}.");
        }
        else
        {
            dialogBox.EnableDialogText(true);
            yield return dialogBox.TypeDialog($"Already reached max workers.");
        }

        yield return new WaitForSeconds(2f);
        yield return ShowStatusChanges(nextOutsideWorker);
        yield return new WaitForSeconds(2.5f);
        PlayerAction();
        yield return new WaitForSeconds(1f);
        PlayerMove();
    }

    void ShowPartyScreen()
    {

        Action onBack = () =>
        {
            state = EntranceState.PlayerAction;
            partyScreen.gameObject.SetActive(false);
            PlayerMove();
        };

        partyScreen.HandleUpdate(onBack);
        
    }

    IEnumerator ShowStatusChanges(Worker worker)
    {
        while (worker.StatusChanges.Count > 0)
        {
            var message = worker.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
}
