using System;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float zoomSpeed = 5000f;
    [SerializeField] private float borderSize = 0.07f;
    [SerializeField] private float maxZoomHeight = 200f;
    [SerializeField] private float minZoomHeight = 15f;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform particleSpawnPoint;
    private bool isRotating = false;
    private Vector3 hitPoint;
    private float rotationSpeed = 1000f;
    private bool allowMovement = true;
    private float cameraHeight = 20f;
    private Vector3 rotatePoint = Vector3.zero;
    private Vector3 hitPointDown = Vector3.zero;
    float targetHeight = 100f;
    float actualZoomHeight = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 15f;
        }
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 1000, UnityEngine.Color.red);

        hitPoint = hit.point;
        Physics.Raycast(Camera.main.transform.position, -Camera.main.transform.up, out RaycastHit hitDown);
        hitPointDown = hitDown.point;
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
            Invoke(nameof(AllowMovementInvoker), 0.5f);
        }
        
        if (isRotating)
        {
            float horizontalMouseInput = Input.GetAxis("Mouse X");
            float verticalMouseInput = Input.GetAxis("Mouse Y");
            verticalMouseInput = Mathf.Clamp(verticalMouseInput, -1f, 1f);
            if (transform.eulerAngles.x < 10)
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

            if (transform.position.y > hitPoint.y + maxZoomHeight)
            {
                zoom = Mathf.Clamp(zoom, 0f, 1f);
                //if (zoom != 0f) 
                //{
                //    zoom = 1.1f;
                //    cameraHeight = cameraHeight * zoom;
                //}
            }
            if (transform.position.y < hitPoint.y + minZoomHeight)
            {
                zoom = Mathf.Clamp(zoom, -1f, 0f);
                //if (zoom != 0f)
                //{
                //    zoom = -1.1f;
                //    cameraHeight = cameraHeight * zoom;
                //}
            }


            bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl);

            bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);

            if (isCtrlPressed && !isShiftPressed)
            {
                actualZoomHeight -= zoomSpeed * 10f * Time.deltaTime;
            }
            else if (isShiftPressed && !isCtrlPressed)
            {
                actualZoomHeight += zoomSpeed * 10f * Time.deltaTime; 
            }

            actualZoomHeight = Mathf.Clamp(actualZoomHeight, minZoomHeight, maxZoomHeight);

            targetHeight = hit.point.y + actualZoomHeight;


            cameraHeight = Mathf.Lerp(cameraHeight, targetHeight, Time.deltaTime * 3f);
            //cameraHeight = Mathf.Clamp(cameraHeight - zoom * zoomSpeed, minZoomHeight, maxZoomHeight);

            transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z);


            //orientation.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            //transform.position = new Vector3(transform.position.x, hitPointDown.y + cameraHeight + 20, transform.position.z);
            transform.position += Time.deltaTime * moveSpeed * orientation.TransformDirection(new Vector3(horizontal * (transform.position.y - hitPoint.y), 0, vertical * (transform.position.y - hitPoint.y)));
            //transform.position += Time.deltaTime * zoomSpeed * transform.TransformDirection(new Vector3(0, 0, zoom * 750));
        }

        orientation.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void AllowMovementInvoker()
    {
        allowMovement = true;
    }

    public Transform GetParticleSpawnPoint() => particleSpawnPoint;
}