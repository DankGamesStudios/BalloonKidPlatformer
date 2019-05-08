using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject player = null;

    [SerializeField] float marginLeft = 10f;
    [SerializeField] float marginRight = 90.0f;
    [SerializeField] float marginDown = 0f;
    [SerializeField] float marginUp = 50f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerPos = player.transform.localPosition;
        float newX = (playerPos.x < marginLeft || playerPos.x > marginRight) ? this.transform.position.x : playerPos.x;
        float newY = (playerPos.y < marginDown || playerPos.y > marginUp) ? this.transform.position.y : playerPos.y;
        this.transform.position = new Vector3(
            newX, newY, this.transform.position.z
        );
    }
}
