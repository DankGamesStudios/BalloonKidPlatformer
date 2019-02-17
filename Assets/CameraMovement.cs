using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject player = null;

    float marginLeft = 0f;
    float marginRight = 65.0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerPos = player.transform.localPosition;
        float newX = (playerPos.x < marginLeft || playerPos.x > marginRight) ? this.transform.position.x : playerPos.x;
        this.transform.position = new Vector3(
            newX, this.transform.position.y, this.transform.position.z
        );
    }
}
