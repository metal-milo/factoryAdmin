using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;

public class WindowGraph : MonoBehaviour
{
    private Sprite dotSprite;
    private RectTransform graphContainer;
    private RectTransform windowGraph;
    private SpriteRenderer sr;
    public Texture2D tex;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    public List<int> valueList = new List<int>() { };

    public Action UpdateGraph;

    private void Awake()
    {
        sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        sr.color = new Color(0f, 0f, 0f, 1.0f);

        //transform.position = new Vector3(1.5f, 1.5f, 0.0f);
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        windowGraph = this.GetComponent<RectTransform>();
        labelTemplateX = windowGraph.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = windowGraph.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = windowGraph.Find("DashTemplateY").GetComponent<RectTransform>();
        dashTemplateY = windowGraph.Find("DashTemplateX").GetComponent<RectTransform>();

        //valueList = new List<int>() {150, 5, 98, 56, 150, 45, 150, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33, 150, 21, 232 };
        //valueList = new List<int>() { 150, 30 };
        //ShowGraph(valueList);
        //CreateHorizontalDash(GetStart(valueList), valueList);
       //createVerticalDash();

    }

    void Start()
    {
        //dotSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.2f, 0.2f), 100.0f);
        Debug.Log("Create sprite");
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void HandleUpdate(Action onBack)
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }

    }


    private GameObject CreateDots(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("dots", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        dotSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.2f, 0.2f), 100.0f);
        sr.sprite = dotSprite;
        gameObject.GetComponent<Image>().sprite = sr.sprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }


    public void ShowGraph(List<int> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        
        float xSize = 25f;
        GameObject lastDotGameObject = null;
        int start = GetStart(valueList);
        float yMaximum = GetMaxValue(start, valueList) + 10;
        for (int i = start; i < valueList.Count; i++)
        {
            float xPosition = xSize + (i - start) * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject dotGameObject = CreateDots(new Vector2(xPosition, yPosition));
            if (lastDotGameObject != null)
            {
                CreateDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, dotGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastDotGameObject = dotGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -1.3f);
            labelX.GetComponent<Text>().text = i.ToString();

        }
        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-15f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();

            
        }

    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 1f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
    }

    public float GetMaxValue(int start, List<int> valueList)
    {
        float maxValue = 0f;
        for (int i = start; i < valueList.Count; i++)
        {
            if (valueList[i] > maxValue)
                maxValue = valueList[i];
        }
        return maxValue;
    }

    public void createVerticalDash()
    {
        float graphHeight = graphContainer.sizeDelta.y;
        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            float normalizedValue = i * 1f / separatorCount;
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-15f, normalizedValue * graphHeight);
        }

    }

    public void CreateHorizontalDash(int start, List<int> valueList)
    {
        float xSize = 25f;
        for (int i = start; i < valueList.Count; i++)
        {
            float xPosition = xSize + (i - start) * xSize;
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -1.3f);
        }
    }

    public int GetStart(List<int> valueList)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float xSize = 25f;
        float dotCount = Mathf.Floor(graphWidth / xSize);
        int start = 0;
        if (valueList.Count > dotCount)
        {
            start = valueList.Count - (int)dotCount;
        }
        return start;
    }

    public void AddValues(List<int> valuesList)
    {
        Clean();
        valueList = valuesList;
        ShowGraph(valueList);
        CreateHorizontalDash(GetStart(valueList), valueList);
        createVerticalDash();
        
    }

    public void Clean()
    {
        foreach (Transform child in labelTemplateX.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in labelTemplateY.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in dashTemplateX.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in dashTemplateY.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in graphContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Destroy(GameObject.Find("dotConnection"));
        Destroy(GameObject.Find("dots"));
        Destroy(GameObject.Find("DashTemplateY(Clone)"));
        Destroy(GameObject.Find("DashTemplateX(Clone)"));
        Destroy(GameObject.Find("LabelTemplateX(Clone)"));
        Destroy(GameObject.Find("LabelTemplateY(Clone)"));
    }
}
