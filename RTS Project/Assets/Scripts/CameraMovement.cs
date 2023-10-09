using System;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float zoomSpeed = 5000f;
    [SerializeField] private float borderSize = 0.07f;
    [SerializeField] private float maxZoomHeight = 50f;
    [SerializeField] private float minZoomHeight = 10f;
    private bool isRotating = false;
    private Vector3 hitPoint;
    private float rotationSpeed = 1000f;
    private bool allowMovement = true;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform particleSpawnPoint;

    private Vector3 rotatePoint = Vector3.zero;


    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //orientation.transform.rotation = Quaternion.Euler(-transform.rotation.x, transform.rotation.y, transform.rotation.z);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 15f;
        }
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit);
        hitPoint = hit.point;
        if (Input.GetMouseButton(2))
        {
            if (!isRotating)
            {
                rotatePoint = hitPoint; 
            }
            isRotating = true;
            allowMovement = false;
        }
        else
        {
            isRotating = false;
            Invoke(nameof(AllowMovementInvoker), 0.75f);
        }
        
        if (isRotating)
        {
            
            float horizontalInput = Input.GetAxis("Mouse X");
            float verticalInput = Input.GetAxis("Mouse Y");
            //print(verticalInput);

            print(transform.eulerAngles.x);
            if (transform.eulerAngles.x < 20)
            {
                verticalInput = Mathf.Clamp(verticalInput, -1f, 0f);
                //Quaternion rotation = transform.rotation;
                //rotation.x = 0.1f;
                //transform.rotation = rotation;
            }
            else if (transform.eulerAngles.x > 60)
            {
                verticalInput = Mathf.Clamp(verticalInput, 0f, 1f);
            }


            //// Clamp the X-axis rotation angle between 20 and 60 degrees
            //float clampedXAngle = Mathf.Clamp(transform.rotation.eulerAngles.x, 20f, 60f);

            //// Create a new Vector3 with the clamped X-axis angle and the original Y and Z angles
            //Vector3 newRotation = new Vector3(clampedXAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            //// Create a new quaternion using the clamped rotation
            //transform.rotation = Quaternion.Euler(newRotation);

            transform.RotateAround(rotatePoint, Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed);
            transform.RotateAround(rotatePoint, Camera.main.transform.right, Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed);
            //transform.RotateAround(rotatePoint, new Vector3(0, 1 * horizontalInput, 1 * verticalInput), rotationSpeed * Time.deltaTime);

            //transform.RotateAround(rotatePoint, Vector3.up, horizontalInput * rotationSpeed);


            Vector3 currentEulerAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(currentEulerAngles.x, currentEulerAngles.y, 0);
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float zoom = Input.GetAxis("Mouse ScrollWheel");

        if (!isRotating && allowMovement)
        {
            //if (Input.mousePosition.y >= Screen.height * (1 - borderSize))
            //{
            //    vertical = 1;
            //}
            //if (Input.mousePosition.y <= Screen.height * borderSize)
            //{
            //    vertical = -1;
            //}
            //if (Input.mousePosition.x >= Screen.width * (1 - borderSize))
            //{
            //    horizontal = 1;
            //}
            //if (Input.mousePosition.x <= Screen.width * borderSize)
            //{
            //    horizontal = -1;
            //}
        }
        if (transform.position.y > hitPoint.y + maxZoomHeight)
        {
            zoom = Mathf.Clamp(zoom, 0f, 1f);
        }
        if (transform.position.y < hitPoint.y + minZoomHeight)
        {
            zoom = Mathf.Clamp(zoom, -1f, 0f);
        }
        
        transform.position += Time.deltaTime * moveSpeed * orientation.TransformDirection(new Vector3(horizontal * (transform.position.y - hitPoint.y), 0, vertical* (transform.position.y - hitPoint.y)));
        transform.position += Time.deltaTime * zoomSpeed * transform.TransformDirection(new Vector3(0, 0, zoom * 750));
    }

    private void AllowMovementInvoker()
    {
        allowMovement = true;
    }

    public Transform GetParticleSpawnPoint() => particleSpawnPoint;
}

//print(transform.rotation.x);

