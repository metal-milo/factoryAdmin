using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> animationSprites;

    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    // States
    SpriteAnimator moveAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    // Refrences
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAnim = new SpriteAnimator(animationSprites, spriteRenderer);

        currentAnim = moveAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;


        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsMoving)
        {
            currentAnim = moveAnim;
            currentAnim.HandleUpdate();
        }
            
        else
            spriteRenderer.sprite = currentAnim.Frames[0];



        wasPreviouslyMoving = IsMoving;
    }


}
