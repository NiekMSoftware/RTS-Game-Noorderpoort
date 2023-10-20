using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerTestMovement : MonoBehaviour
{
    public float speed;
    public bool PlayerMoving = false;
    private Transform playerTransform;
    private Vector3 previousPosition;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        previousPosition = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        x *= speed * Time.deltaTime;
        z *= speed * Time.deltaTime;

        transform.Translate(x,0,z);
        if (playerTransform.position != previousPosition)
        {
            print("Player is moving");
            PlayerMoving = true;
        }   
        previousPosition = playerTransform.position;
    }
}
