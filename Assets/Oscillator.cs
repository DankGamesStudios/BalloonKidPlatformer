using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 0f, 0f);
    [SerializeField] float period = 2f;
    
    // move character with platform
    private GameObject player = null;
    // internet implementation
    // private Vector3 playerOffset;
    private Vector3 offset;

    // todo remove from inspector later
    [Range(0,1)]
    float movementFactor; // 0 for not moved, 1 for fully moved

    Vector3 startingPos;
    Vector3 oscillatorOffset;

    bool doneThisFrame = true;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        player = null;
    }

    private void OnTriggerStay2D(Collider2D other) {
        player = other.gameObject;
        oscillatorOffset = transform.position - startingPos;
        // internet implementation
        // playerOffset = player.transform.position - transform.position;
    }

    private void OnTriggerExit2D(Collider2D other) {
        player = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
        {
            return;
        }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;

        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = rawSinWave / 2f + 0.5f;

        offset = movementFactor * movementVector;
        transform.position = startingPos + offset;

        doneThisFrame = false;
    }

    private void LateUpdate() {
        // internet implementation
        // if (player != null) {
        //      player.transform.position = transform.position + playerOffset;
        // }
        if (player != null && !doneThisFrame) {
            //print("offset on trigger " + oscillatorOffset + ", offset now " + offset + ", player position" + player.transform.position);
            player.transform.position += (offset - oscillatorOffset);
            doneThisFrame = true;
        }
    }
}
