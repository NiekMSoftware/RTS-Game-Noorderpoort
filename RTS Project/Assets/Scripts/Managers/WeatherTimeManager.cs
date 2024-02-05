using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class WeatherTimeManager : MonoBehaviour
{
    [Header("Lighting Settings")]
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

    [Header("Wind settings")]
    [SerializeField] WindZone wind;
    [SerializeField] float windDirectionChangeSpeed;

    [Header("Cloud Settings")]
    [SerializeField] GameObject clouds;
    [SerializeField] private float cloudSpeedModifier;
    [SerializeField] float cloudRotationSpeed;

    [Header("Weather Settings")]
    [SerializeField] private bool autoChangeWeather;
    [SerializeField] private Vector2 minMaxTimePerWeather;
    [SerializeField] private WeatherStates currentWeather;
    [SerializeField] private Weather[] weathers;

    [System.Serializable]
    public struct Weather
    {
        public ParticleSettings[] particlesSettings;
        public CloudSettings cloudState;
        public float windSpeed;
        public AudioManager.AudioGroupNames audioToPlay;
    }

    [System.Serializable]
    public struct CloudSettings
    {
        public float density;
        public Color color;
    }

    [System.Serializable]
    public struct ParticleSettings
    {
        public ParticleSystem particleSystem;
        public float particleAmountMultiplier;
    }

    public enum WeatherStates
    {
        None,
        Rain,
        HeavyRain,
        Snowing,
        Storm
    }

    private Camera mainCamera;

    private bool isDay;
    private float currentSpeed;

    private List<ParticleSystem> weatherParticles = new();

    private void Start()
    {
        mainCamera = Camera.main;
        UpdateWeather();
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

            //ChangeWind();


        }

        UpdateLightning(timeOfDay / 24);
    }

    private void ChangeWind()
    {
        Vector2 windMoveDirection = new Vector2(Random.Range(-1 * windDirectionChangeSpeed, 1 * windDirectionChangeSpeed), Random.Range(-1 * windDirectionChangeSpeed, 1 * windDirectionChangeSpeed));

        Quaternion targetRotation = Quaternion.Euler(windMoveDirection);

        wind.transform.Rotate(windMoveDirection * cloudSpeedModifier);
        Quaternion rotation = Quaternion.Slerp(wind.transform.rotation, targetRotation, cloudRotationSpeed * Time.deltaTime);
        rotation.x = 180;
        rotation.z = 0;
        wind.transform.rotation = rotation;

        print(rotation);
    }

    private void UpdateWeather()
    {
        if (!Application.isPlaying) return;

        Weather weather = weathers[(int)currentWeather];

        if (weatherParticles.Count > 0)
        {
            foreach (var particle in weatherParticles)
            {
                Destroy(particle);
            }
        }

        if (weather.particlesSettings.Length > 0)
        {
            foreach (var particle in weather.particlesSettings)
            {
                GameObject weatherParticle = Instantiate(particle.particleSystem.gameObject, mainCamera.GetComponent<CameraMovement>().GetParticleSpawnPoint());

                if (weatherParticle.TryGetComponent(out ParticleSystem particleSystem))
                {
                    weatherParticles.Add(particleSystem);
                    ParticleSystem.EmissionModule emission = particleSystem.emission;
                    emission.rateOverTimeMultiplier = particle.particleAmountMultiplier;
                    particleSystem.Play();
                }
            }
        }

        wind.GetComponent<WindZone>().windMain = weather.windSpeed;
        clouds.GetComponent<MeshRenderer>().material.SetColor("_Color2", weather.cloudState.color);
        clouds.GetComponent<MeshRenderer>().material.SetFloat("_Density", weather.cloudState.density);
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


#if UNITY_EDITOR
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
#endif
}