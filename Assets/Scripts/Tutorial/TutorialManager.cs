using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject doorTutorial;
    public GameObject[] popUps;
    private int popIndex;
    public float waitTime = 5f;

    private void Awake()
    {
        popIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (popIndex == popUps.Length - 1)
        {
            SceneManager.LoadScene(0);
        }
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popIndex)
            {
                popUps[i].SetActive(true);
            }
            else
            {
                popUps[i].SetActive(false);
            }
        }
        if (popIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                popIndex++;
            }
        }
        else if (popIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                popIndex++;
            }
        }
        else if (popIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                popIndex++;
            }
        }
        else if (popIndex == 3)
        {
            doorTutorial.SetActive(true);
            if (waitTime <= 0)
            {
                popIndex++;
                waitTime = 5f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
            

        }
        else if (popIndex == 4)
        {
            if (waitTime <= 0)
            {
                popIndex++;
                waitTime = 5f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }


        }
        else if (popIndex == 5)
        {
            if (waitTime <= 0)
            {
                popIndex++;
                waitTime = 5f;
                doorTutorial.SetActive(false);
            }
            else
            {
                waitTime -= Time.deltaTime;
            }


        }
        else if (popIndex == 6)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                popIndex++;
            }


        }
    }
}
