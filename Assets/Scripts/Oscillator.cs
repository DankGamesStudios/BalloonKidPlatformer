using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 0f, 0f);
    [SerializeField] float period = 2f;
    

    // todo remove from inspector later
    [Range(0,1)]
    float movementFactor; // 0 for not moved, 1 for fully moved

    // move character with platform
    private GameObject player = null;
    private Vector3 startingPos;
    private Vector3 previousOffset = new Vector3(0f, 0f, 0f);
    private Vector3 playerPosition;
    private Vector3 offset = new Vector3(0f, 0f, 0f);

    bool doneThisFrame = true;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        player = null;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        player = other.gameObject;
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

        previousOffset = offset; // set previous offset
        offset = movementFactor * movementVector;
        transform.position = startingPos + offset;

        doneThisFrame = false;
    }

    private void LateUpdate() {
        if (player != null)
        {
            if (!doneThisFrame)
            {
                doneThisFrame = true;
                playerPosition = player.transform.position;
            }
            // while there is a player on platform, translate its position with
            // the difference between updates of the oscillator
            player.transform.position = playerPosition + (offset - previousOffset);
        }
    }
}
