using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviourScript : MonoBehaviour
{
    // maxSpeed pare destul de rapid la 1x
    [SerializeField] public float maxSpeed = 1.0f;
    bool facingRight = true;

    Animator anim;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(move));
        AnimatorStateInfo playingAnimation = anim.GetCurrentAnimatorStateInfo(0);
        // nush ce face velocity aici, i am sorry
        // rb.velocity = new Vector2(rb.velocity.x + move * maxSpeed, rb.velocity.y);
        // pot sa astept dupa animatie sa ma misc
        if (playingAnimation.IsName("walking")) // like ffs what logic in naming methods is this???
        {
            transform.position = new Vector3(transform.position.x + move * maxSpeed, transform.position.y, transform.position.z);
        }
        // sau pot sa testez move si sa dau play la animatia de walking
        // (dar nush daca trece prin tranzitia definita intre idle si walking)
        // if (!playingAnimation.IsName("walking") && Mathf.Abs(move) > 0.01 )
        // {
        //     anim.Play("walking", 0, 0);
        // }
        // transform.position = new Vector3(transform.position.x + move * maxSpeed, transform.position.y, transform.position.z); 
 
        if (move > 0 && !facingRight)
        {
            FlipFacing();
        }
        else if(move < 0 && facingRight)
        {
            FlipFacing();
        }
    }

    void FlipFacing()
    {
        facingRight = !facingRight;
        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }
}


