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
    [SerializeField] private Transform particleSpawnPoint;

    private void Start()
    {
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
            //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, hitPoint.y + 10, hitPoint.y + 50), transform.position.z);
        }
        if (transform.position.y > hitPoint.y + 50)
        {
            zoom = Mathf.Clamp(zoom, 0f, 1f);
        }
        if (transform.position.y < hitPoint.y + 10)
        {
            zoom = Mathf.Clamp(zoom, -1f, 0f);
        }
        transform.position += orientation.TransformDirection(new Vector3(horizontal * (transform.position.y - hitPoint.y), 0, vertical* (transform.position.y - hitPoint.y))) * moveSpeed * Time.deltaTime;
        transform.position += transform.TransformDirection(new Vector3(0, 0, zoom * 750)) * zoomSpeed * Time.deltaTime;
    }

    private void AllowMovementInvoker()
    {
        allowMovement = true;
    }

    public Transform GetParticleSpawnPoint() => particleSpawnPoint;
}
