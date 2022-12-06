using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Free, Door, Dialog, Paused, Menu, Inventory, PartyScreen, Shop, Working, Rating, SuperStars, Graph, Marketing }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] DoorSystem doorSystem;
    [SerializeField] Camera playerCamera;
    [SerializeField] InteractableLight interactableLight;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] RatingUI ratingUI;
    [SerializeField] SuperStarsUI superStarsUI;
    [SerializeField] WindowGraph graphUI;
    [SerializeField] MarketingUI marketingUI;
    [SerializeField] GameObject productionStatus;




    public static GameController Instance { get; private set; }

    MenuController menuController;
    TimeController timeController;
    GraphController graphController;
    Marketplace marketplace;
    RatingController ratingController;
    DaysFader daysFader;
    FinishDayFader finishDayFader;
    BandController[] bandControllers;
    List<WorkerCharacter> workerCharacters = new List<WorkerCharacter>();
    GameObject[] gos;

    GameState state;
    GameState prevState;
    bool working;
    bool isStartDay = false;
    bool isFadeOutDay = false;
    bool isEndDay = false;
    bool isFadeOutFinishDay = false;
    bool restartDay = false;
    bool isGameOver = false;
    public float waitTime = 1.5f;
    public float waitEndDay = 5f;
    public float waitNextDay = 1.5f;
    int actualDay=1;
    float initialMoney;
    float spentMoney;
    float sales;
    float totalEndDay;
    int salaries;

    int finalHour = 12;
    int finalMinute = 59;

    bool eventActive = false;
    float eventDuration = 0f;
    EventBase activeEvent;

    List<Worker> outsideWorkers = new List<Worker>();
    public GameState State => state;

    private void Awake()
    {
        
        Instance = this;
        menuController = GetComponent<MenuController>();
        timeController = GetComponent<TimeController>();
        ratingController = GetComponent<RatingController>();
        marketplace = GetComponent<Marketplace>();
        daysFader = FindObjectOfType<DaysFader>();
        finishDayFader = FindObjectOfType<FinishDayFader>();
        graphController = FindObjectOfType<GraphController>();
        bandControllers = FindObjectsOfType<BandController>();
        gos = GameObject.FindGameObjectsWithTag("Worker");
        foreach (GameObject go in gos)
        {
            workerCharacters.Add(go.GetComponent<WorkerCharacter>());
        }
        

        ItemDB.Init();
        RateDB.Init();
        WorkerDB.Init();
        MoodDB.Init();
        QuestDB.Init();
        EventDB.Init();


        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Dictionary<string, WorkerBase> objects = WorkerDB.objects;
        foreach (var objectWorker in objects)
        {
            var outsideWorkerCopy = new Worker(objectWorker.Value, 11);
            outsideWorkerCopy.SetStatus(MoodId.happy);
            outsideWorkers.Add(outsideWorkerCopy);
        }


    }

    private void Start()
    {
        
        working = false;
        interactableLight.OnDoor += StartShift;
        doorSystem.EntranceOver += EndEntrance;

        partyScreen.Init();
        timeController.Init();
        timeController.SetTime(7, 30);

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            if(state == GameState.Dialog)
            {
                state = prevState;
            }
            
        };

        menuController.onBack += () =>
        {
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
        };

        graphController.onBackGraph += () =>
        {
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
        };

        Event.OnTimeEvent += TriggerEvent;

        menuController.onMenuSelected += OnMenuSelected;

        graphController.onGraphSelected += OnGraphSelected;

        ShopController.i.OnStart += () => state = GameState.Shop;
        if (working)
            ShopController.i.OnFinish += () => state = GameState.Working;
        else
            ShopController.i.OnFinish += () => state = GameState.Free;

        Debug.Log("Awake:" + SceneManager.GetActiveScene().name);

        if (SceneManager.GetActiveScene().name != "Tutorial")
            isStartDay = true;
        //StartCoroutine(daysFader.FadeOut(0.5f));
        marketplace.totalDefectives = 0;
        productionStatus.gameObject.SetActive(false);
        foreach (WorkerCharacter workCharact in workerCharacters)
        {
            workCharact.Visible(false);
        }
    }

    
    void StartShift()
    {
        state = GameState.Door;
        doorSystem.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        var workerParty = playerController.GetComponent<WorkerParty>();
        //var outsideWorker = FindObjectOfType<humanR>().GetComponent<humanR>().GetRandomWorker(0);
        //var outsideWorkerCopy = new Worker(outsideWorker.Base, outsideWorker.Level);
        doorSystem.SelectStaff(workerParty, outsideWorkers);
    }

    
    void EndEntrance()
    {
        state = GameState.Free;
        doorSystem.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        interactableLight.gameObject.SetActive(false);
        productionStatus.gameObject.SetActive(true);
        timeController.SetTime(7, 59);
        working = true;
        state = GameState.Working;
        foreach (BandController item in bandControllers)
        {
            item.StartMoving();
        }
        var workerParty = playerController.GetComponent<WorkerParty>();
        foreach (WorkerCharacter workCharact in workerCharacters)
        {
            
            workCharact.Visible(true);
        }
        
    }

    void InitializeWorkers()
    {
        var workerParty = playerController.GetComponent<WorkerParty>();
        workerParty.ClearWorkers();
        foreach (var worker in outsideWorkers)
        {
            worker.Init();
        }
    }

    void GetEndDayNumbers()
    {
        spentMoney = Money.i.MoneySpent;
        sales = Money.i.MoneyFrmSales;
        var workerParty = playerController.GetComponent<WorkerParty>();
        int totalSalaries = 0;
        foreach (var worker in workerParty.Workers){
            totalSalaries += (worker.Salary * 8);
            worker.SetStatus(GetNextMoodEnum(worker.Status.Id));
        }
        salaries = totalSalaries;
        Money.i.TakeMoney(totalSalaries);
        totalEndDay = initialMoney + sales - spentMoney - salaries;
        Money.i.RestartMoney();
        InitializeWorkers();
        foreach (BandController item in bandControllers)
        {
            item.StopMoving();
        }
        foreach (WorkerCharacter workCharact in workerCharacters)
        {
            
            workCharact.Visible(false);
            
        }
    }

    public void TriggerEvent(EventBase possibleEvent)
    {
        Debug.Log($"Time Event {possibleEvent.Name}");
        if (possibleEvent.CompleteDay)
        {
            timeController.SetTime(finalHour, finalMinute);
        }
        else
        {
            int actualHour = TimeController.Hour;
            int actualMin = TimeController.Minute;
            int sumHour = actualHour + (possibleEvent.AffectTime / 60);
            int sumMin = actualMin + (possibleEvent.AffectTime % 60);
            timeController.SetTime(sumHour, sumMin);
            string message = possibleEvent.Name;
            StartCoroutine(daysFader.FadeIn(0.5f,message));
            isFadeOutDay = true;
        }
        
    }

    private void Update()
    {

        if (isFadeOutDay)
        {
            waitTime -= Time.deltaTime;
        }
        if (waitTime < 0)
        {
            waitTime = 1.5f;
            isFadeOutDay = false;
            StartCoroutine(daysFader.FadeOut(0.5f));
        }

        if (isStartDay)
        {
            
            initialMoney = Money.i.MoneyQuantity;
            string message = "Day " + actualDay;
            StartCoroutine(daysFader.FadeIn(0.5f,message));
            isStartDay = false;
            isFadeOutDay = true;
        }


        if (isFadeOutFinishDay)
        {
            waitEndDay -= Time.deltaTime;
        }
        if (waitEndDay < 0)
        {
            isFadeOutFinishDay = false;
            waitEndDay = 3.5f;
            
            StartCoroutine(finishDayFader.FadeOut(0.5f));
            interactableLight.gameObject.SetActive(true);
            productionStatus.gameObject.SetActive(false);
            //
            isGameOver = ValidateGameOver();
            isStartDay = !ValidateGameOver();
            
            state = GameState.Free;
        }

        if (isEndDay)
        {
            isEndDay = false;
            state = GameState.Paused;
            
            StartCoroutine(finishDayFader.FadeIn(0.5f, (int)initialMoney, (int)spentMoney, (int)sales, salaries, (int)totalEndDay));

            
            isFadeOutFinishDay = true;
            

        }

        if (isGameOver)
        {
            StartCoroutine(daysFader.GameOver(0.5f));
            state = GameState.Paused;
        }

        if (state == GameState.Free)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Door)
        {
            doorSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.Inventory)
        {
            ShowInventoryUI();
            
        }
        else if (state == GameState.PartyScreen)
        {
            ShowPartyScreen();

        }
        else if (state == GameState.Shop)
        {
            ShopController.i.HandleUpdate();
        }
        else if (state == GameState.Working)
        {
            playerController.HandleUpdate();
            timeController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Rating)
        {
            ShowRatingUI();

        }

        else if (state == GameState.SuperStars)
        {
            ShowStarsUI();

        }
        else if (state == GameState.Graph)
        {
            graphController.HandleUpdate();

        }
        else if (state == GameState.Marketing)
        {
            ShowMarketingUI();

        }

        if (TimeController.Hour >= finalHour && TimeController.Minute >= finalMinute)
        {
            timeController.SetTime(7, 30);
            GetEndDayNumbers();
            isEndDay = true;
            actualDay++;

        }

    }

    public bool ValidateGameOver()
    {
        if (totalEndDay < 0)
            return true;
        else
            return false;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }

    void OnMenuSelected(int selectedMenuItem)
    {
        if (selectedMenuItem == 0)
        {
            //party
            OpenPartyScreen();
        }
        else if (selectedMenuItem == 1)
        {
            //inventory
            OpenInventoryUI();
        }
        else if (selectedMenuItem == 2)
        {
            //rating
            OpenRatingUI();
        }
        else if (selectedMenuItem == 3)
        {
            //super stars
            OpenStarsUI();
        }
        else if (selectedMenuItem == 4)
        {
            //graph
            graphController.OpenGraphsMenu();
            state = GameState.Graph;
        }
        else if (selectedMenuItem == 5)
        {
            //Marketing
            OpenMarketingUI();
        }
        else if (selectedMenuItem == 6)
        {
            //save
            SavingSystem.i.Save("saveSlot1");
            state = GameState.Free;
        }
        else if (selectedMenuItem == 7)
        {
            //load
            SavingSystem.i.Load("saveSlot1");
            state = GameState.Free;
        }
        else if (selectedMenuItem == 8)
        {
            //exit
            SceneManager.LoadScene(0);
        }

        //state = GameState.Free;
    }

    void OnGraphSelected(int selectedMenuItem)
    {
        if (selectedMenuItem == 0)
        {
            //customers
            OpenGraphUI(marketplace.customers);
        }
        else if (selectedMenuItem == 1)
        {
            //sales
            OpenGraphUI(marketplace.sales);
        }
        else if (selectedMenuItem == 2)
        {
            //defects
            OpenGraphUI(marketplace.defs);
        }
        

        //state = GameState.Free;
    }

    void OpenPartyScreen()
    {
        Debug.Log("Party Screen");
        state = GameState.PartyScreen;
        partyScreen.SetPartyData(playerController.GetComponent<WorkerParty>().Workers);
        partyScreen.gameObject.SetActive(true);
    }

    void OpenInventoryUI()
    {
        inventoryUI.gameObject.SetActive(true);
        state = GameState.Inventory;
    }

    void ShowPartyScreen()
    {

        Action onBack = () =>
        {
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
            partyScreen.gameObject.SetActive(false);
        };

        partyScreen.HandleUpdate(onBack);

    }

    void ShowInventoryUI()
    {
        Action onBack = () =>
        {
            inventoryUI.gameObject.SetActive(false);
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
        };

        inventoryUI.HandleUpdate(onBack);

    }

    void OpenRatingUI()
    {
        ratingUI.Show();
        state = GameState.Rating;
    }

    void OpenStarsUI()
    {
        superStarsUI.Show();
        state = GameState.SuperStars;
    }
    
    void OpenMarketingUI()
    {
        marketingUI.Show();
        state = GameState.Marketing;
    }

    void OpenGraphUI(List<int> graph)
    {
        graphUI.Show();
        
        graphUI.AddValues(graph);
        state = GameState.Graph;
    }

    void ShowRatingUI()
    {
        Action onBack = () =>
        {
            ratingUI.gameObject.SetActive(false);
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
        };

        ratingUI.HandleUpdate(onBack);

    }

    void ShowStarsUI()
    {
        Action onBack = () =>
        {
            superStarsUI.Hide();
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
        };

        superStarsUI.HandleUpdate(onBack);

    }
    
    void ShowMarketingUI()
    {
        Action onBack = () =>
        {
            marketingUI.Hide();
            if (working)
                state = GameState.Working;
            else
                state = GameState.Free;
        };

        marketingUI.HandleUpdate(onBack);

    }

    MoodId GetNextMoodEnum(MoodId moodId)
    {
        MoodId returnMood = MoodId.great;
        if (moodId == MoodId.quietquit)
        {
            returnMood = MoodId.quietquit;
        }
        else
        {
            foreach (int i in Enum.GetValues(typeof(MoodId)))
            {
                if (moodId == (MoodId)i)
                {
                    returnMood = (MoodId)(i + 1);
                }
                Console.WriteLine(((MoodId)i));
            }
        }
        
        return returnMood;

    }

    private int RandomNumber(int minLimit, int maxLimit)
    {
        return UnityEngine.Random.Range(minLimit, maxLimit);
    }
}
