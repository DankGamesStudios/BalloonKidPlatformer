using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class BigKidMovement : MonoBehaviour
{
    [SerializeField] float horizontalModificator = 5;
    [SerializeField] float balloonModificator = 5;
    [SerializeField] float jumpModificator = 5;
    [SerializeField] float gravity = 20f;
    [SerializeField] int   blinkPause = 15;
    [SerializeField] float yWhenStanding = -1.96f;
    public GameObject Balloon;

    enum Animations {Jump, Fall, Run, Blink, None};
    Animations playing;
    bool playerLock = false;
    [SerializeField] float balloonTime = 5f;
    float startBalloonTime = 0f;
    bool balloonAlive = false;

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
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
            print("Reload!!!");
            Invoke("LoadV2", 0.5f);
        }
        if (!playerLock)
        {
            MovementOnInput();
            BalloonActions();
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
                    winMessage.transform.position = new Vector3(
                        winMessage.transform.position.x,
                        0.27f,
                        winMessage.transform.position.z);
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
        // SceneManager.LoadScene(0);
    }

    private void LoadV2()
    {
        // basically reload
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private void BalloonActions() {
        if (CrossPlatformInputManager.GetButton("Fire3")){
            if (balloonAlive) {
                //print("ballon already alive, destroying");
                DestroyBalloon();
            }
            startBalloonTime = Time.realtimeSinceStartup;
            balloonAlive = true;
            //print("create balloon " + startBalloonTime);
            //print("expect finnish at " + (startBalloonTime + balloonTime));
            GameObject balloon = Instantiate(Balloon, transform);
            balloon.transform.position += new Vector3(0f, 0f, 0f);
            balloon.name = "Balloon";
        }

        if (startBalloonTime > 0 && Time.realtimeSinceStartup > startBalloonTime + balloonTime)
        {
            //print("destroy balloon");
            DestroyBalloon();
        }
    }

    private void DestroyBalloon() {
        GameObject balloon = GameObject.Find("Balloon");
        if (balloon != null) {
            Destroy(balloon);
            startBalloonTime = 0f;
        }
        balloonAlive = false;
    }

    private void MovementOnInput()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        kidRenderer.flipX = horizontal < 0;

        float horizontalChange = horizontal * horizontalModificator * Time.deltaTime;
        float verticalChange = 0; // not jumping now
        bool isJumping = CrossPlatformInputManager.GetButton("Jump");
        if (isJumping)
        {
            if (balloonAlive)
            {
                float jump = balloonModificator * Time.deltaTime;
                rigidBody.freezeRotation = true;
                rigidBody.AddRelativeForce(Vector3.up * jump);
                rigidBody.freezeRotation = false;
            }
            else // jump with gravity
            {
                // muahahah math !!!
                verticalChange = (jumpModificator - gravity * Time.deltaTime) * Time.deltaTime;
            }
        }
        transform.position = new Vector3(
            transform.position.x + horizontalChange,
            transform.position.y + verticalChange,
            transform.position.z);
        
        Animate(horizontal, isJumping);
    }

    private void Animate(float horizontal, bool isJumping)
    {
        bool isFalling = transform.position.y > yWhenStanding && CrossPlatformInputManager.GetButton("Jump") == false;
        bool isWalking = Mathf.Abs(horizontal) > Mathf.Epsilon && !isFalling;
        bool shouldBlink = Mathf.CeilToInt(Time.realtimeSinceStartup) % blinkPause == 0;

        if (isJumping)
        {
            // animator.Play("Run - Basic");
            animator.speed = 1;
            playing = Animations.Jump;
        }
        else if (isFalling)
        {
            // animator.Play("Run - Basic");
            animator.speed = 1;
            playing = Animations.Fall;
        }
        else if (isWalking)
        {
            // animator.Play("Run - Basic");
            //print("am i running?");
            animator.speed = 1;
            playing = Animations.Run;
        }
        else if (shouldBlink)
        {
            if (playing != Animations.Blink)
            {
                // animator.Play("Stand - Basic");
                animator.speed = 1;
                playing = Animations.Blink;
                Invoke("StopAnimator", 5f);
            }
        }
        else //stop immediately
        {
            //print("playing " + playing);
            if (playing != Animations.Blink)
            {
                // animator.Play("Stand - Basic");
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
