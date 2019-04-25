using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MovementBehaviourScript : MonoBehaviour
{
    // maxSpeed pare destul de rapid la 1x
    [SerializeField] public float maxSpeed = 1.0f;
    [SerializeField] float balloonTime = 5f;
    [SerializeField] float balloonModificator = 600;
    [SerializeField] float jumpModificator = 5;
    [SerializeField] float gravity = 20f;
    bool facingRight = true;
    public GameObject Balloon;
    float startBalloonTime = 0f;
    bool balloonAlive = false;
    bool triggerBalloonAnimation = false;

    Animator anim;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void BalloonActions()
    {
        if (CrossPlatformInputManager.GetButton("Fire3"))
        {
            if (balloonAlive)
            {
                DestroyBalloon();
            }
            startBalloonTime = Time.realtimeSinceStartup;
            balloonAlive = true;
            GameObject balloon = Instantiate(Balloon, transform);
            balloon.transform.position += new Vector3(0f, 0f, 0f);
            balloon.name = "Balloon";
            triggerBalloonAnimation = true;
        }

        if (startBalloonTime > 0 && Time.realtimeSinceStartup > startBalloonTime + balloonTime)
        {
            DestroyBalloon();
        }

        anim.SetBool("IsBalloonAlive", triggerBalloonAnimation);
        if (triggerBalloonAnimation == true)
        {
            triggerBalloonAnimation = false; // send 
        }
    }

    private void DestroyBalloon()
    {
        GameObject balloon = GameObject.Find("Balloon");
        if (balloon != null)
        {
            Destroy(balloon);
            startBalloonTime = 0f;
        }
        balloonAlive = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float move = CrossPlatformInputManager.GetAxis("Horizontal");
        bool isJumping = CrossPlatformInputManager.GetButton("Jump");
        float horizontalChange = 0f;
        float verticalChange = 0f; // not jumping now
        anim.SetFloat("Speed", Mathf.Abs(move));
        anim.SetBool("IsJumping", isJumping);
        BalloonActions();
        AnimatorStateInfo playingAnimation = anim.GetCurrentAnimatorStateInfo(0);
        // nush ce face velocity aici, i am sorry
        // rb.velocity = new Vector2(rb.velocity.x + move * maxSpeed, rb.velocity.y);
        // pot sa astept dupa animatie sa ma misc
        if (playingAnimation.IsName("walking")) // like ffs what logic in naming methods is this???
        {
            horizontalChange = move * maxSpeed;
        }
        if (isJumping)
        {
            verticalChange = JumpOnInput();
            anim.SetFloat("Speed", 0f); //no walking when jumping
        }
        transform.position = new Vector3(
            transform.position.x + horizontalChange,
            transform.position.y + verticalChange,
            transform.position.z);

        if (move > 0 && !facingRight)
        {
            FlipFacing();
        }
        else if(move < 0 && facingRight)
        {
            FlipFacing();
        }
    }

    private float JumpOnInput()
    {
        float verticalChange = 0f; // not jumping now
        if (balloonAlive)
        {
            rb.freezeRotation = true;
            rb.AddRelativeForce(Vector3.up * balloonModificator);
            rb.freezeRotation = false;
        }
        else // jump with gravity
        {
            // muahahah math !!!
            verticalChange = (jumpModificator - gravity);
        }
        return verticalChange;
    }

    void FlipFacing()
    {
        facingRight = !facingRight;
        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }
}


