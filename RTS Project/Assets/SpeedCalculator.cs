using UnityEngine;

public class SpeedCalculator : MonoBehaviour
{
    private Vector3 lastPosition;
    private float lastTime;

    private void Start()
    {
        // Initialize the last position and time when the object starts.
        lastPosition = transform.position;
        lastTime = Time.time;
    }

    private void Update()
    {
        // Calculate the current position and time.
        Vector3 currentPosition = transform.position;
        float currentTime = Time.time;

        // Calculate the distance traveled since the last update.
        float distance = Vector3.Distance(currentPosition, lastPosition);

        // Calculate the time elapsed since the last update.
        float deltaTime = currentTime - lastTime;

        // Calculate the speed using the formula: Speed = Distance / Time
        float speed = distance / deltaTime;

        // Update the last position and time for the next frame.
        lastPosition = currentPosition;
        lastTime = currentTime;

        // Display the speed in the Unity console.
        Debug.Log("Speed: " + speed + " units per second");
    }
}
