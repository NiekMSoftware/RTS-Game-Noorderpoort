using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
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
    [SerializeField] private float zoomAngle;

    private Rigidbody rb;

    private Vector3 input;

    private Vector2 rotationInput;
    private Vector3 currentRotation;

    private float zoomInput;
    private Vector3 cameraDirection => transform.InverseTransformDirection(cameraHolder.forward);

    private Quaternion finalRotation;
    private bool hasRotated;

    private Transform orientation;
    private Transform zoomOrientation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.inertiaTensor = Vector3.one;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        orientation = new GameObject().transform;
        orientation.name = "Orientation";

        zoomOrientation = new GameObject().transform;
        zoomOrientation.name = "ZoomOrientation";
    }

    private void Update()
    {
        HandleInput();

        CheckBounds();

        RotateOrientation();

        Movement();

        Zoom();

        debugText.text = transform.forward.ToString();
        currentRotation = Vector3.Lerp(currentRotation, rotationInput, Time.deltaTime * rotationSmoothing);

        if (transform.forward.y < rotationRangeY.x)
        {
            print("too low");
            Vector3 newRotation = transform.forward;
            newRotation.y = rotationRangeY.x + 0.01f;
            transform.forward = newRotation;
        }
        else if (transform.forward.y > rotationRangeY.y)
        {
            print("too high");
            Vector3 newRotation = transform.forward;
            newRotation.y = rotationRangeY.y - 0.01f;
            transform.forward = newRotation;
        }

        Vector3 rotation = new(currentRotation.y, currentRotation.x);
        finalRotation = Quaternion.Euler(rotation);

        if (hasRotated)
        {
            print("reset rotation stuff");
            //Reset z rotation
            Vector3 eulerAngles = transform.eulerAngles;
            transform.eulerAngles = new(eulerAngles.x, eulerAngles.y, 0);

            currentRotation = Vector3.zero;
            rotationInput = Vector2.zero;
            hasRotated = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Rotation();
    }

    private void RotateOrientation()
    {
        orientation.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        zoomOrientation.rotation = Quaternion.Euler(zoomAngle, transform.eulerAngles.y, 0);
    }

    private void Zoom()
    {
        if (transform.position.y > zoomRange.y)
        {
            rb.velocity = Vector3.zero;
            Vector3 position = transform.position;
            position.y = zoomRange.y - 0.1f;
            transform.position = position;
            zoomInput = Mathf.Clamp(zoomInput, 0, 1);
        }
        else if (transform.position.y < zoomRange.x)
        {
            rb.velocity = Vector3.zero;
            Vector3 position = transform.position;
            position.y = zoomRange.x + 0.1f;
            transform.position = position;
            zoomInput = Mathf.Clamp(zoomInput, -1, 0);
        }

        if (zoomInput != 0 && ZoomIsInBounds(cameraHolder.localPosition))
        {
            rb.AddForce(Time.deltaTime * zoomInput * zoomSmoothing * zoomOrientation.forward, ForceMode.Force);
        }
    }

    private bool ZoomIsInBounds(Vector3 position)
    {
        return position.magnitude > zoomRange.x && position.magnitude < zoomRange.y;
    }

    private void Rotation()
    {
        rb.MoveRotation(rb.rotation * finalRotation);
        //rb.rotation = finalRotation;
        hasRotated = true;
    }

    private void Movement()
    {
        if (input.magnitude >= 0.1f)
        {
            print("hi");
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
