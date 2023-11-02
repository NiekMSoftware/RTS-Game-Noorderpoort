using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private Vector2 range;

    private Rigidbody rb;

    private Vector3 input;

    private Vector2 rotationInput;
    private Vector3 currentRotation;

    private bool isColliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();

        CheckBounds();

        Movement();

        Rotation();

        //Vector3 zoomDir = new Vector3
    }

    private void Rotation()
    {
        currentRotation = Vector3.Lerp(currentRotation, rotationInput, Time.deltaTime * rotationSmoothing);

        //transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        rb.AddTorque(transform.up * currentRotation.x * rotationSpeed, ForceMode.Force);
        rb.AddTorque(-transform.right * currentRotation.y * rotationSpeed, ForceMode.Force);

        currentRotation = Vector3.zero;
        rotationInput = Vector2.zero;
        //Vector3 torque = new(currentRotation.x, currentRotation.y, 0);
        //rb.AddRelativeTorque(torque, ForceMode.VelocityChange);

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
        input = new(Input.GetAxisRaw("Horizontal"), Input.mouseScrollDelta.y, Input.GetAxisRaw("Vertical"));

        Vector3 right = transform.right * input.x;
        Vector3 forward = transform.forward * input.z;

        input = (right + forward).normalized;

        //Rotation Input
        if (!Input.GetMouseButton(2)) return;

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

    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
}
