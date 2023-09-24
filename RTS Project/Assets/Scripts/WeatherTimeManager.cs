using UnityEngine;

[ExecuteAlways]
public class WeatherTimeManager : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private float speed;
    [Range(0, 24)]
    [SerializeField] private float timeOfDay;
    [Range(4, 8)]
    [SerializeField] private int dayBegin;
    [Range(17, 21)]
    [SerializeField] private int dayEnd;

    [SerializeField] Gradient ambientColor;
    [SerializeField] Gradient fogColor;
    [SerializeField] Gradient directionalColor;

    [SerializeField] WindZone wind;
    [SerializeField] float windDirectionChangeSpeed;
    [SerializeField] GameObject clouds;
    [SerializeField] float cloudSpeedModifier;
    [SerializeField] float cloudRotationSpeed;

    private bool isDay;

    private void Update()
    {
        if (timeOfDay < dayBegin || timeOfDay > dayEnd)
        {
            isDay = false;
        }
        else
        {
            isDay = true;
        }

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * speed;
            timeOfDay %= 24;

            clouds.GetComponent<MeshRenderer>().material.SetVector("_ScrollDirection", new Vector2(wind.transform.rotation.x, wind.transform.rotation.z));
            clouds.GetComponent<MeshRenderer>().material.SetFloat("_Speed", wind.windMain * cloudSpeedModifier);

            Vector2 windMoveDirection;

            windMoveDirection = new Vector2(Random.Range(-1 * windDirectionChangeSpeed, 1 * windDirectionChangeSpeed), Random.Range(-1 * windDirectionChangeSpeed, 1 * windDirectionChangeSpeed));

            Quaternion targetRotation = Quaternion.Euler(windMoveDirection);

            //wind.transform.Rotate(windMoveDirection * cloudSpeedModifier);
            Quaternion rotation = Quaternion.Slerp(wind.transform.rotation, targetRotation, cloudRotationSpeed * Time.deltaTime);
            rotation.x = 180;
            rotation.z = 0;
            wind.transform.rotation = rotation;

            print(rotation);
        }

        UpdateLightning(timeOfDay / 24);
    }

    private void UpdateLightning(float timePercent)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = fogColor.Evaluate(timePercent);
        
        sun.color = directionalColor.Evaluate(timePercent);
        sun.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0f));
    }

    public bool IsDay() => isDay;
}