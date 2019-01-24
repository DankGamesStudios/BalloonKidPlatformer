using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class KidMovement : MonoBehaviour
{
    [SerializeField] float horizontalModificator = 5;
    [SerializeField] float verticalModificator = 5;
    [SerializeField] int blinkPause = 15;

    enum Animations {Jump, Fall, Run, Blink, None};
    Animations playing;

    Rigidbody2D rigidBody = null;
    Animator animator = null;

    // Start is called before the first frame update
    void Start()
    {
        print("hello!");
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playing = Animations.Blink;
    }

    // Update is called once per frame
    void Update()
    {
        MovementOnInput();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collided with: " + collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "environment":
                break;
            case "death":
                print("lost, restarting");
                Invoke("LoadFirstLevel", 0.75f);
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
        print("horizontal " + horizontal);
        bool isJumping = CrossPlatformInputManager.GetButton("Jump");
        if (isJumping)
        {
            float jump = verticalModificator * Time.deltaTime;
            rigidBody.freezeRotation = true;
            rigidBody.AddRelativeForce(Vector3.up * jump);
            rigidBody.freezeRotation = false;
        }
        print("jump " + isJumping);
        transform.localPosition = new Vector3(
            transform.localPosition.x + horizontal * horizontalModificator * Time.deltaTime,
            transform.localPosition.y,
            transform.localPosition.z);

        Animate(horizontal, isJumping);
    }

    private void Animate(float horizontal, bool isJumping)
    {
        bool isFalling = transform.localPosition.y > -3.3 && CrossPlatformInputManager.GetButtonUp("Jump") == true;
        bool isWalking = Mathf.Abs(horizontal) > Mathf.Epsilon && !isFalling;

        if (isJumping)
        {
            print("animate jump up");
            animator.Play("kid jump up");
            animator.speed = 1;
            playing = Animations.Jump;
        }
        else if (isFalling)
        {
            print("animate jump down");
            animator.Play("kid jump down");
            animator.speed = 1;
            playing = Animations.Fall;
        }
        else if (isWalking)
        {
            print("animate walk");
            animator.Play("kid walk");
            animator.speed = 1;
            playing = Animations.Run;
        }
        else
        {
            bool shouldBlink = Mathf.CeilToInt(Time.realtimeSinceStartup) % blinkPause == 0;
            if (shouldBlink)
            {
                print("animate blink");
                animator.Play("kid idle");
                animator.speed = 1;
                playing = Animations.Blink;
            }
            else if (playing == Animations.Blink) //give blink time to finish
            {
                print("will stop animations in 1.5 seconds");
                Invoke("StopAnimator", 1.5f);
            }
            else //stop immediately
            {
                print("freeze");
                animator.speed = 0;
            }
        }
    }

    private void StopAnimator()
    {
        print("stop animator");
        playing = Animations.None;
        animator.speed = 0;
    }
}
