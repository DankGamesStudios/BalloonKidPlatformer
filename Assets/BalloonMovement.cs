using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class BalloonMovement : MonoBehaviour
{
    [SerializeField] float rotationWhenFlying = -45;
    [SerializeField] float rotationWhenWalking = 15;
    [SerializeField] float horizontalModificator = 2;
    [SerializeField] float ropeLength = 0.5f;
    [SerializeField] float balloonHelium = 1f;

    bool tiedToPlayer = true;
    float yWhenStanding = -2.25f;

    Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("balloon collided with: " + collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "unlock":
                print("Finish unlocked");
                Destroy(collision.gameObject); //bye bye mushroom
                collision.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                transform.parent.gameObject.SendMessage("UnlockFinish");
                break;
            default:
                break;
        }
    }
        // Update is called once per frame
    void Update()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float horizontalChange = horizontal * horizontalModificator * Time.deltaTime;
        bool isFiring = CrossPlatformInputManager.GetButton("Fire1");
        bool isFlying = transform.parent.position.y > yWhenStanding;

        if (isFiring)
        {
            print("release the balloon");
            tiedToPlayer = false;
        }
        if (tiedToPlayer)
        {
            if (isFlying)
            {
                transform.rotation = Quaternion.Euler(0, 0, horizontal * rotationWhenFlying);
                float xpos = transform.localPosition.x + horizontalChange;
                float range = ropeLength + origin.x;
                float horizontalShift = Mathf.Clamp(xpos, -range, range);
                transform.localPosition = new Vector3(
                    horizontalShift,
                    origin.y,
                    origin.z);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0,  horizontal * rotationWhenWalking);
                transform.localPosition = new Vector3(
                    Mathf.Clamp(transform.localPosition.x, -origin.x, origin.x),
                    Mathf.Clamp(transform.localPosition.y, -origin.y, origin.y),
                    origin.z);
            }
        }
        else
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y + balloonHelium * Time.deltaTime,
                transform.localPosition.z
                );
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 10f * Mathf.Sin(10 * Time.time)));
            if (transform.localPosition.y > 7f)
            {
                print("balloon destroyed");
                transform.localScale = new Vector3(0f, 0f, 0f); //poof
            }
        }
    }
}
