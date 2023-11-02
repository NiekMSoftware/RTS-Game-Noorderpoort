using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothing;
    [SerializeField] private Vector2 range;

    private Rigidbody rb;

    private Vector3 input;

    private float targetAngle;
    private float currentAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();

        CheckBounds();

        Movement();

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotationSmoothing);
        transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);

        //Vector3 zoomDir = new Vector3
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
        if (!Input.GetMouseButtonDown(2)) return;
        targetAngle += Input.GetAxisRaw("Mouse X") * rotationSpeed;
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