using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class KidMovement : MonoBehaviour
{
    [SerializeField] float horizontalModificator = 5;
    [SerializeField] float balloonRotation = 100;
    [SerializeField] float balloonHelium = 1f;
    [SerializeField] float verticalModificator = 5;
    [SerializeField] int   blinkPause = 15;

    enum Animations {Jump, Fall, Run, Blink, None};
    Animations playing;
    bool hasBalloonControl = true;
    bool playerLock = false;

    Rigidbody2D rigidBody = null;
    Transform balloonTransform = null;
    Animator animator = null;
    Vector3 balloonOrigin;
    GameObject winMessage = null;

    // Start is called before the first frame update
    void Start()
    {
        print("hello!");
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        balloonTransform = transform.GetChild(0);
        balloonOrigin = balloonTransform.localPosition;
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
        print("collided with: " + collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "environment":
                break;
            case "death":
                print("lost, restarting");
                Invoke("LoadFirstLevel", 0.75f);
                break;
            case "Finish":
                print("You won");
                playerLock = true;
                animator.Play("kid idle");
                animator.speed = 0;
                winMessage.transform.localPosition = new Vector3(
                    winMessage.transform.localPosition.x,
                    0.25f, //winMessage.transform.localPosition.y ,
                    winMessage.transform.localPosition.z);
                // how to print message on screen?
                Invoke("LoadFirstLevel", 5f);
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
        float horizontalChange = horizontal * horizontalModificator * Time.deltaTime;
        transform.localPosition = new Vector3(
            transform.localPosition.x + horizontalChange,
            transform.localPosition.y,
            transform.localPosition.z);
        if (hasBalloonControl)
        {
            if (transform.localPosition.y > -3.2)
            {
                balloonTransform.rotation = Quaternion.Euler(0, 0, horizontal * balloonRotation);
                float xpos = balloonTransform.localPosition.x + horizontalChange;
                float range = 0.5f + balloonOrigin.x;
                float balloonHorizontalShift = Mathf.Clamp(xpos, -range, range);
                balloonTransform.localPosition = new Vector3(
                    balloonHorizontalShift,
                    balloonOrigin.y,
                    balloonOrigin.z);
            }
            else
            {
                balloonTransform.rotation = Quaternion.Euler(0, 0, 0);
                balloonTransform.localPosition = new Vector3(
                    Mathf.Clamp(balloonTransform.localPosition.x, -balloonOrigin.x, balloonOrigin.x),
                    Mathf.Clamp(balloonTransform.localPosition.y, -balloonOrigin.y, balloonOrigin.y),
                    balloonOrigin.z);
            }
        }

        bool isJumping = CrossPlatformInputManager.GetButton("Jump");
        if (isJumping)
        {
            float jump = verticalModificator * Time.deltaTime;
            rigidBody.freezeRotation = true;
            rigidBody.AddRelativeForce(Vector3.up * jump);
            rigidBody.freezeRotation = false;
        }

        bool isFiring = CrossPlatformInputManager.GetButton("Fire1");
        if (isFiring)
        {
            print("release the balloon");
            hasBalloonControl = false;
        }
        if (!hasBalloonControl)
        {
            balloonTransform.localPosition = new Vector3(
                balloonTransform.localPosition.x,
                balloonTransform.localPosition.y + balloonHelium * Time.deltaTime,
                balloonTransform.localPosition.z
                );
            balloonTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 10f * Mathf.Sin(10*Time.time)));
            if (balloonTransform.localPosition.y > 7f)
            {
                print("balloon destroyed");
                balloonTransform.localScale = new Vector3(0f, 0f, 0f); //poof
            }
        }
        Animate(horizontal, isJumping);
    }

    private void Animate(float horizontal, bool isJumping)
    {
        bool isFalling = transform.localPosition.y > -3.2 && CrossPlatformInputManager.GetButton("Jump") == false;
        bool isWalking = Mathf.Abs(horizontal) > Mathf.Epsilon && !isFalling;
        bool shouldBlink = Mathf.CeilToInt(Time.realtimeSinceStartup) % blinkPause == 0;

        if (isJumping)
        {
            animator.Play("kid jump up");
            animator.speed = 1;
            playing = Animations.Jump;
        }
        else if (isFalling)
        {
            animator.Play("kid jump down");
            animator.speed = 1;
            playing = Animations.Fall;
        }
        else if (isWalking)
        {
            animator.Play("kid walk");
            animator.speed = 1;
            playing = Animations.Run;
        }
        else if (shouldBlink)
        {
            if (playing != Animations.Blink)
            {
                animator.Play("kid idle");
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
                animator.Play("kid idle");
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
}
