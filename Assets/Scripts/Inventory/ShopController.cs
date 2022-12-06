using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState { Menu, Buying, Selling, Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] MoneyUI moneyUI;
    [SerializeField] CountSelectorUI countSelectorUI;

    public event Action OnStart;
    public event Action OnFinish;

    ShopState state;

    InteractablePc interactablePc;

    public static ShopController i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(InteractablePc interactablePc)
    {
        this.interactablePc = interactablePc;

        OnStart?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("What do you want to do?",
            waitForInput: false,
            choices: new List<string>() { "Buy", "Sell", "Quit" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            // Buy
            state = ShopState.Buying;
            moneyUI.Show();
            shopUI.Show(interactablePc.RawMaterials, (item) => StartCoroutine(BuyItem(item)), OnBackFromBuying);
        }
        else if (selectedChoice == 1)
        {
            // Sell
            state = ShopState.Selling;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2)
        {
            // Quit
            OnFinish?.Invoke();
            yield break;
        }
    }

    
    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling, (selectedItem) => StartCoroutine(SellItem(selectedItem)));
        }
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }

    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("Testing no sellable");
            state = ShopState.Selling;
            yield break;
        }
        else
        {
            moneyUI.Show();

            float sellingPrice = Mathf.Round(item.Price / 2);
            int countToSell = 1;

            int itemCount = inventory.GetItemCount(item);
            if (itemCount > 1)
            {
                yield return DialogManager.Instance.ShowDialogText("How many would you like to sell?",
                    waitForInput: false, autoClose: false);

                yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
                    (selectedCount) => countToSell = selectedCount);

                DialogManager.Instance.CloseDialog();
            }

            sellingPrice = sellingPrice * countToSell;

            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Sell for {sellingPrice}?",
                waitForInput: false,
                choices: new List<string>() {"Yes", "No" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                inventory.RemoveItem(item, countToSell);
                Money.i.AddMoney(sellingPrice);
                yield return DialogManager.Instance.ShowDialogText($"Turned over {item.Name} and received {sellingPrice}!");
            }

            moneyUI.Close();
        }

        state = ShopState.Selling;
    }


    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;

        yield return DialogManager.Instance.ShowDialogText("How many would you like to buy?",
            waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Money.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"That will be {totalPrice}",
                waitForInput: false,
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                // Selected Yes
                inventory.AddItem(item, countToBuy);
                Money.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText("Thank you for shopping with us!");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("Not enough money for that!");
        }

        state = ShopState.Buying;
    }

    void OnBackFromBuying()
    {
        shopUI.Close();
        moneyUI.Close();
        StartCoroutine(StartMenuState());
    }

}
