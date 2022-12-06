using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] Color highlatedColor;
    [SerializeField] MoneyUI moneyUI;

    public event Action<int> onMenuSelected;
    public event Action onBack;

    List<Text> menuItems;

    int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }

    public void OpenMenu()
    {
        moneyUI.Show();
        menu.SetActive(true);
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        moneyUI.Close();
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.S))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.W))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseMenu();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
                menuItems[i].color = highlatedColor;
            else
                menuItems[i].color = Color.black;
        }
    }
}
