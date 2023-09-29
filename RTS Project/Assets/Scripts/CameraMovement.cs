using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float zoomSpeed = 5000f;
    [SerializeField] private float borderSize = 0.07f;
    private bool isRotating = false;
    private Vector3 hitPoint;
    private float rotationSpeed = 3.5f;
    private bool allowMovement = true;
    [SerializeField] private Transform orientation;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 15f;
        }
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
            Invoke(nameof(AllowMovementInvoker), 0.75f);
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
        if (transform.position.y > hitPoint.y + 50)
        {
            zoom = Mathf.Clamp(zoom, 0f, 1f);
        }
        if (transform.position.y < hitPoint.y + 10)
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
}
