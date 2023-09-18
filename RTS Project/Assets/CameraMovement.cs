using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 5000f;
    [SerializeField] private float borderSize = 0.07f;
    private Camera mainCamera;
    public bool isRotating = false;
    private Vector3 hitPoint;
    private float rotationSpeed = 3.5f;
    public bool allowMovement = true;
    [SerializeField] private Transform orientation;

    private void Start()
    {
        this.mainCamera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit);
        hitPoint = hit.point;
        if (Input.GetMouseButton(2))
        {

                isRotating = true;
                allowMovement = false;
        }
        else
        {
            isRotating = false;
            Invoke("AllowMovementInvoker", 0.75f);
        }
        
        if (isRotating)
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            transform.RotateAround(hitPoint, Vector3.up, horizontalInput * rotationSpeed);

            Vector3 currentEulerAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(currentEulerAngles);
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float zoom = Input.GetAxis("Mouse ScrollWheel");

        if (!isRotating && allowMovement)
        {
            if (Input.mousePosition.y >= Screen.height * (1 - borderSize))
            {
                vertical = 1;
            }
            if (Input.mousePosition.y <= Screen.height * borderSize)
            {
                vertical = -1;
            }
            if (Input.mousePosition.x >= Screen.width * (1 - borderSize))
            {
                horizontal = 1;
            }
            if (Input.mousePosition.x <= Screen.width * borderSize)
            {
                horizontal = -1;
            }
            transform.position += orientation.TransformDirection(new Vector3(horizontal, 0, vertical)) * moveSpeed * Time.deltaTime;
            if (transform.position.y < hitPoint.y + 50 && zoom < 0)
            {
                transform.position += transform.TransformDirection(new Vector3(0, 0, zoom * 500)) * zoomSpeed * Time.deltaTime;
            }
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, hitPoint.y + 10, hitPoint.y + 50), transform.position.z);



            print(hitPoint);

        }
    }
    private void AllowMovementInvoker()
    {
        allowMovement = true;
    }
}
