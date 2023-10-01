using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[ExecuteAlways]
public class WeatherTimeManager : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private float daySpeed;
    [SerializeField] private float nightSpeed;
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
    [SerializeField] private float cloudSpeedModifier;
    [SerializeField] float cloudRotationSpeed;

    [SerializeField] private WeatherStates currentWeather;
    [SerializeField] private Weather[] weathers;


    [System.Serializable]
    public class Weather
    {
        public GameObject weatherParticle;
        public float particlesAmountMultiplier;
        public CloudSettings cloudState;
        public float windSpeed;
        public AudioManager.AudioGroupNames audioToPlay;
    }

    [System.Serializable]
    public class CloudSettings
    {
        public float density;
        public Color color;
    }

    public enum WeatherStates
    {
        None,
        Rain,
        HeavyRain
    }

    private Camera mainCamera;

    private bool isDay;
    private float currentSpeed;

    private GameObject weatherParticle;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (timeOfDay < dayBegin || timeOfDay > dayEnd)
        {
            currentSpeed = nightSpeed;
            isDay = false;
        }
        else
        {
            currentSpeed = daySpeed;
            isDay = true;
        }

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime * currentSpeed;
            timeOfDay %= 24;

            clouds.GetComponent<MeshRenderer>().material.SetVector("_ScrollDirection", new Vector2(wind.transform.rotation.x, wind.transform.rotation.z));
            clouds.GetComponent<MeshRenderer>().material.SetFloat("_Speed", wind.windMain * cloudSpeedModifier);

            Vector2 windMoveDirection;

            windMoveDirection = new Vector2(Random.Range(-1 * windDirectionChangeSpeed, 1 * windDirectionChangeSpeed), Random.Range(-1 * windDirectionChangeSpeed, 1 * windDirectionChangeSpeed));

            Quaternion targetRotation = Quaternion.Euler(windMoveDirection);

            //wind.transform.Rotate(windMoveDirection * cloudSpeedModifier);
            //Quaternion rotation = Quaternion.Slerp(wind.transform.rotation, targetRotation, cloudRotationSpeed * Time.deltaTime);
            //rotation.x = 180;
            //rotation.z = 0;
            //wind.transform.rotation = rotation;

            //print(rotation);
        }

        UpdateLightning(timeOfDay / 24);
    }

    private void UpdateWeather()
    {
        if (!Application.isPlaying) return;

        Weather weather = weathers[(int)currentWeather];

        if (weather.weatherParticle)
        {
            if (weatherParticle)
            {
                Destroy(weatherParticle);
            }

            weatherParticle = Instantiate(weather.weatherParticle, mainCamera.GetComponent<CameraMovement>().GetParticleSpawnPoint());
            ParticleSystem particleSystem = weatherParticle.GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = weather.particlesAmountMultiplier;
            particleSystem.GetComponent<ParticleSystem>().Play();
        }
        else if (weatherParticle)
        {
            Destroy(weatherParticle);
        }

        wind.GetComponent<WindZone>().windMain = weather.windSpeed;
        clouds.GetComponent<MeshRenderer>().material.SetColor("_Color2", weather.cloudState.color);
        clouds.GetComponent<MeshRenderer>().material.SetFloat("_Density", weather.cloudState.density);
        print(clouds.GetComponent<MeshRenderer>().material.GetFloat("_Density"));
        AudioManager.Instance.PlaySounds(AudioManager.Instance.audioGroups[(int)weather.audioToPlay], true, true);
    }

    private void UpdateLightning(float timePercent)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = fogColor.Evaluate(timePercent);
        
        sun.color = directionalColor.Evaluate(timePercent);
        sun.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0f));
    }

    public bool IsDay() => isDay;

    [CustomEditor(typeof(WeatherTimeManager))]
    class WeatherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WeatherTimeManager script = (WeatherTimeManager)target;
            if (GUILayout.Button("Update Weather"))
            {
                script.UpdateWeather();
            }
        }
    }
}