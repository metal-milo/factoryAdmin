using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject customer;
    [SerializeField] private Vector2 spawnPosition;
    float spawn = 0f;
    Scene currentScene;


    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        Marketplace.instantiateCust += OnSpawnCustomer;
    }

    private void OnDisable()
    {
        Marketplace.instantiateCust -= OnSpawnCustomer;
    }

    
    private void Update()
    {
        /*
        spawn += Time.deltaTime;

        if (spawn > 5)
        {
            spawn = 0;
            OnSpawnCustomer();
        }*/
    }


    public void OnSpawnCustomer()
    {
        Instantiate(customer, spawnPosition, Quaternion.identity);
    }

    
}
