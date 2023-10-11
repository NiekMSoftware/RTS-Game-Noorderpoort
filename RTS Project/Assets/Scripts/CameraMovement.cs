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
            float horizontalMouseInput = Input.GetAxis("Mouse X");
            float verticalMouseInput = Input.GetAxis("Mouse Y");

            if (transform.eulerAngles.x < 20)
            {
                verticalMouseInput = Mathf.Clamp(verticalMouseInput, 0f, 1f);
            }
            else if (transform.eulerAngles.x > 60)
            {
                verticalMouseInput = Mathf.Clamp(verticalMouseInput, -1f, 0f);

            }

            transform.RotateAround(rotatePoint, Vector3.up, horizontalMouseInput * Time.deltaTime * rotationSpeed);
            transform.RotateAround(rotatePoint, Camera.main.transform.right, verticalMouseInput * Time.deltaTime * rotationSpeed);

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