using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float borderSize = 0.07f;
    private Camera mainCamera;

    private void Start()
    {
        this.mainCamera = GetComponent<Camera>();
    }
    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 1000, Color.yellow);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out RaycastHit hit))
        {
            print(hit.point);
            //hit.point 
        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float zoom = Input.GetAxis("Mouse ScrollWheel");

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
        transform.position += new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - zoom * zoomSpeed, 2, 15);
    }
}
