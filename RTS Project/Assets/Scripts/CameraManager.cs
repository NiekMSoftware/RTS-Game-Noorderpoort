using TMPro;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform zoomOrientation;
    [SerializeField] private TMP_Text debugText;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 range;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private Vector2 rotationRangeY;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float zoomSmoothing;
    [SerializeField] private Vector2 zoomRange;

    private Rigidbody rb;

    private Vector3 input;

    private Vector2 rotationInput;
    private Vector3 currentRotation;

    private float zoomInput;
    private Vector3 cameraDirection => transform.InverseTransformDirection(cameraHolder.forward);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.inertiaTensor = Vector3.one;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleInput();

        CheckBounds();

        RotateOrientation();

        Movement();

        Rotation();

        Zoom();
    }

    private void RotateOrientation()
    {
        orientation.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void Zoom()
    {
        if (transform.position.y > zoomRange.y)
        {
            zoomInput = Mathf.Clamp(zoomInput, 0, 1);
        }
        else if (transform.position.y < zoomRange.x)
        {
            zoomInput = Mathf.Clamp(zoomInput, -1, 0);
        }

        if (zoomInput != 0 && ZoomIsInBounds(cameraHolder.localPosition))
        {
            rb.AddForce(Time.deltaTime * zoomInput * zoomSmoothing * zoomOrientation.forward, ForceMode.Force);
        }
    }

    private bool ZoomIsInBounds(Vector3 position)
    {
        print(position.magnitude);
        print(position.magnitude > zoomRange.x && position.magnitude < zoomRange.y);
        return position.magnitude > zoomRange.x && position.magnitude < zoomRange.y;
    }

    private void Rotation()
    {
        //print(-transform.right);
        debugText.SetText(transform.up.ToString());

        if (transform.up.y <= rotationRangeY.x)
        {
            print("too high");
            //rotationInput.y = Mathf.Clamp(rotationInput.y, -1, 0);
        }

        if (transform.up.y >= rotationRangeY.y)
        {
            print("too low");
            //rotationInput.y = Mathf.Clamp(rotationInput.y, 0, 1);
        }

        //currentRotation = Vector3.Lerp(currentRotation, rotationInput, Time.deltaTime * rotationSmoothing);
        currentRotation = rotationInput;

        rb.AddRelativeTorque(currentRotation.x * rotationSpeed * transform.up, ForceMode.Force);
        //rb.AddRelativeTorque(currentRotation.y * rotationSpeed * -transform.right, ForceMode.Force);

        Vector3 eulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, 0);

        currentRotation = Vector3.zero;
        rotationInput = Vector2.zero;
    }

    private void Movement()
    {
        if (input.magnitude >= 0.1f)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * input, ForceMode.Force);
        }
    }

    private void HandleInput()
    {
        //Movement Input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 right = orientation.right * x;
        Vector3 forward = orientation.forward * z;

        input = (right + forward).normalized;

        //Zoom Input
        zoomInput = Input.mouseScrollDelta.y;

        //Rotation Input
        if (!Input.GetMouseButton(1)) return;

        rotationInput += new Vector2(Input.GetAxisRaw("Mouse X") * rotationSpeed, Input.GetAxisRaw("Mouse Y") * rotationSpeed);
    }

    private void CheckBounds()
    {
        Vector3 pos = transform.position;

        if (pos.x < -range.x) pos.x = -range.x;
        if (pos.x > range.x) pos.x = range.x;
        if (pos.z < -range.y) pos.z = -range.y;
        if (pos.z > range.y) pos.z = range.y;

        transform.position = pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 5f);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(range.x * 2f, 5f, range.y * 2f));
    }
}
