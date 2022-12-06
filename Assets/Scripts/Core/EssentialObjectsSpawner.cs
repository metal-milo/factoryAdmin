using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            var spawnPos = new Vector3(0, 0, 0);
            Instantiate(essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }

   
}

