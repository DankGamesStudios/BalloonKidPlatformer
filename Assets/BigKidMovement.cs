using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class BigKidMovement : MonoBehaviour
{
    [SerializeField] float horizontalModificator = 5;
    [SerializeField] float verticalModificator = 5;
    [SerializeField] int   blinkPause = 15;

    enum Animations {Jump, Fall, Run, Blink, None};
    Animations playing;
    bool playerLock = false;

    Rigidbody2D rigidBody = null;
    SpriteRenderer kidRenderer = null;
    Animator animator = null;
    GameObject winMessage = null;

    public bool finishLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        print("hello!");
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        kidRenderer = GetComponent<SpriteRenderer>();
        playing = Animations.Blink;
        winMessage = GameObject.Find("Win Message");
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerLock)
        {
            MovementOnInput();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerLock)
        {
            return;
        }
        //print("collided with: " + collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "environment":
                break;
            case "death":
                print("lost, restarting");
                Invoke("LoadFirstLevel", 0.75f);
                break;
            case "Finish":
                if (!finishLocked)
                {
                    print("You won");
                    playerLock = true;
                    animator.Play("Stand - Basic");
                    animator.speed = 0;
                    winMessage.transform.localPosition = new Vector3(
                        winMessage.transform.localPosition.x,
                        0.27f,
                        winMessage.transform.localPosition.z);
                    // how to print message on screen?
                    Invoke("Quit", 3f);
                }
                break;
            default:
                break;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void MovementOnInput()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        kidRenderer.flipX = horizontal < 0;

        float horizontalChange = horizontal * horizontalModificator * Time.deltaTime;
        transform.localPosition = new Vector3(
            transform.localPosition.x + horizontalChange,
            transform.localPosition.y,
            transform.localPosition.z);
        bool isJumping = CrossPlatformInputManager.GetButton("Jump");
        if (isJumping)
        {
            float jump = verticalModificator * Time.deltaTime;
            rigidBody.freezeRotation = true;
            rigidBody.AddRelativeForce(Vector3.up * jump);
            rigidBody.freezeRotation = false;
        }
        Animate(horizontal, isJumping);
    }

    private void Animate(float horizontal, bool isJumping)
    {
        bool isFalling = transform.localPosition.y > -1.96 && CrossPlatformInputManager.GetButton("Jump") == false;
        bool isWalking = Mathf.Abs(horizontal) > Mathf.Epsilon && !isFalling;
        bool shouldBlink = Mathf.CeilToInt(Time.realtimeSinceStartup) % blinkPause == 0;

        if (isJumping)
        {
            animator.Play("Run - Basic");
            animator.speed = 1;
            playing = Animations.Jump;
        }
        else if (isFalling)
        {
            animator.Play("Run - Basic");
            animator.speed = 1;
            playing = Animations.Fall;
        }
        else if (isWalking)
        {
            animator.Play("Run - Basic");
            print("am i running?");
            animator.speed = 1;
            playing = Animations.Run;
        }
        else if (shouldBlink)
        {
            if (playing != Animations.Blink)
            {
                animator.Play("Stand - Basic");
                animator.speed = 1;
                playing = Animations.Blink;
                Invoke("StopAnimator", 5f);
            }
        }
        else //stop immediately
        {
            print("playing " + playing);
            if (playing != Animations.Blink)
            {
                animator.Play("Stand - Basic");
                playing = Animations.None;
                animator.speed = 0;
            }
        }
    }

    private void StopAnimator()
    {
        playing = Animations.None;
        animator.speed = 0;
    }

    private void Quit()
    {
        Application.Quit();
    }
}
