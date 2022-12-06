using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandController : MonoBehaviour
{
    public float moveSpeed;

    public bool isProduction { get; set; }

    private Animator animator;

    public float OffsetY { get; private set; } = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame

    private void Update()
    {
        animator.SetBool("IsProduction", isProduction);
    }
    

    public void StopMoving()
    {
        isProduction = false;
    }

    public void StartMoving()
    {
        isProduction = true;
    }

}
