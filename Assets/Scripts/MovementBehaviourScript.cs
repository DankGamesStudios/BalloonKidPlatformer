using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviourScript : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    bool facingRight = true;

    Animator anim;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(move));
        rb.velocity = new Vector2(rb.velocity.x + move * maxSpeed, rb.velocity.y);


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


