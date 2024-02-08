using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] private float cameraFPS;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90, player.eulerAngles.y, 0);
    }

    float elapsed = 0;

    protected virtual void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 1 / cameraFPS)
        {
            elapsed = 0;
            cam.Render();
        }
    }
}
