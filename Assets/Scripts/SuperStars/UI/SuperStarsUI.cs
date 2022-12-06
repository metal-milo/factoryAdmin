using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SuperStarsUIState { StarSelection, Busy }
public class SuperStarsUI : MonoBehaviour
{
    [SerializeField] GameObject starsList;
    [SerializeField] StarItemUI starSlotUI;
    [SerializeField] Image starIcon;
    [SerializeField] Text starDescription;
    [SerializeField] MoneyUI moneyUI;

    List<StarItemUI> slotUIList;
    int selectedStar = 0;

    SuperStars superStars;
    RectTransform starListRect;
    Marketplace marketplace;
    Produce production;

    Action<StarBase> onStarCalled;
    SuperStarsUIState state;
    const int itemsInViewPort = 8;
    private void Awake()
    {
        superStars = SuperStars.GetStars();
        starListRect = starsList.GetComponent<RectTransform>();
        marketplace = FindObjectOfType<GameController>().GetComponent<Marketplace>();
        production = FindObjectOfType<GameController>().GetComponent<Produce>();
    }

    public void Show()
    {
        moneyUI.Show();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        moneyUI.Close();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateStarsList();
        superStars.OnUpdated += UpdateStarsList;
    }

    void UpdateStarsList()
    {
        foreach (Transform child in starsList.transform)
        {
            Destroy(child.gameObject);
        }


        slotUIList = new List<StarItemUI>();
        foreach (var starSlot in superStars.GetSlots())
        {
            var slotUIObj = Instantiate(starSlotUI, starsList.transform);
            slotUIObj.SetData(starSlot);

            slotUIList.Add(slotUIObj);

        }

        UpdateStarSelection();

    }
    public void HandleUpdate(Action onBack, Action<StarBase> onStarCalled = null)
    {
        this.onStarCalled = onStarCalled;
        if (state == SuperStarsUIState.StarSelection)
        {
            int prevSelection = selectedStar;

            if (Input.GetKeyDown(KeyCode.S))
                ++selectedStar;
            else if (Input.GetKeyDown(KeyCode.W))
                --selectedStar;

            selectedStar = Mathf.Clamp(selectedStar, 0, superStars.GetSlots().Count - 1);


            if (prevSelection != selectedStar)
            {
                UpdateStarSelection();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(StarSelected());
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }

        }
    }

    void UpdateStarSelection()
    {
        var slots = superStars.GetSlots();

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedStar)
            {
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                slotUIList[i].NameText.color = Color.black;
            }

        }

        selectedStar = Mathf.Clamp(selectedStar, 0, slots.Count - 1);

        if (slots.Count > 0)
        {
            var star = slots[selectedStar].Star;
            starIcon.sprite = star.Icon;
            starDescription.text = star.Description;
        }

        HandleScrolling();

    }

    void HandleScrolling()
    {
        if (selectedStar >= 0)
        {
            float scrollPos = Mathf.Clamp(selectedStar - (itemsInViewPort / 2), 0, selectedStar) * slotUIList[0].Height;
            starListRect.localPosition = new Vector2(starListRect.localPosition.x, scrollPos);
        }


    }

    IEnumerator StarSelected()
    {
        state = SuperStarsUIState.Busy;

        var star = superStars.GetStar(selectedStar);
        
        if (!star.CanUse)
        {
            yield return DialogManager.Instance.ShowDialogText($"This super star cannot be called right now");
            state = SuperStarsUIState.StarSelection;
            yield break;
        }
        
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"Do you want to call {star.Name}?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
        if (selectedChoice == 0)
        {

            if (Money.i.HasMoney(star.Price))
            {

                StarBase callStar;
                if (superStars.IsMarketStar(selectedStar))
                {
                    callStar = superStars.CallSuperStarMarket(selectedStar, marketplace);
                }
                else
                {
                    callStar = superStars.CallSuperStarProd(selectedStar, production);
                }

                if (callStar != null)
                {
                    Money.i.TakeMoney(star.Price);
                    yield return DialogManager.Instance.ShowDialogText($"{star.Name} will join for today");
                    onStarCalled?.Invoke(star);
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"Star already working");
                }
            }
            else
            {
                yield return DialogManager.Instance.ShowDialogText("Not enough money for that!");
            }

            
        }
        state = SuperStarsUIState.StarSelection;


    }


}
