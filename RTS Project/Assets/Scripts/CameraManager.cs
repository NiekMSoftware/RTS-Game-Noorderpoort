using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horInput = Input.GetAxisRaw("Horizontal");
        float verInput = Input.GetAxisRaw("Vertical");

        Vector3 movementDir = new Vector3(horInput, 0, verInput).normalized;

        if (movementDir.magnitude >= 0.1f)
        {
            print(movementDir * moveSpeed * Time.deltaTime);
            rb.AddForce(moveSpeed * Time.deltaTime * movementDir);
        }
    }
}