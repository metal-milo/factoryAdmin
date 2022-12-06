using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    [SerializeField] GameObject graphsMenu;
    [SerializeField] GameObject graph;
    [SerializeField] Color highlatedColor;

    public event Action<int> onGraphSelected;
    public event Action onBackGraph;

    List<Text> menuItems;

    int selectedItem = 0;

    private void Awake()
    {
        menuItems = graphsMenu.GetComponentsInChildren<Text>().ToList();
    }

    public void OpenGraphsMenu()
    {
        graphsMenu.SetActive(true);
        UpdateGraphSelection();
    }

    public void CloseGraphsMenu()
    {
        graphsMenu.SetActive(false);
    }

    public void CloseGraph()
    {
        graph.SetActive(false);
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
            UpdateGraphSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onGraphSelected?.Invoke(selectedItem);
            CloseGraphsMenu();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBackGraph?.Invoke();
            CloseGraphsMenu();
            CloseGraph();
        }
    }

    void UpdateGraphSelection()
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
