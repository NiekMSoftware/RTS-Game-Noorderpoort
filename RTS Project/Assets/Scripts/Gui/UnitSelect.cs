using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private RawImage icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private Button locationButton;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private Camera mainCamera;

    private Unit currentUnit;
    private NavMeshAgent agent;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));

        locationButton.onClick.RemoveAllListeners();
        locationButton.onClick.AddListener(GoToDestination);
    }

    private void GoToDestination()
    {
        mainCamera.transform.position = agent.destination;
    }

    private void Setup()
    {
        if (currentUnit == null) return;

        agent = currentUnit.GetComponent<NavMeshAgent>();
        icon.texture = currentUnit.GetRenderTexture();
        nameText.SetText(currentUnit.name);
        if (agent.destination == currentUnit.transform.position || agent.isStopped == true || agent.velocity == Vector3.zero)
        {
            locationText.gameObject.SetActive(false);
        }
        else
        {
            locationText.gameObject.SetActive(true);

            if (currentUnit.GetDestination() == null)
            {
                locationText.SetText("Going to: Target Position");
            }
            else
            {
                locationText.SetText("Going to: " + currentUnit.GetDestination().name);
            }
        }
    }

    private void Update()
    {
        if (!currentUnit) return;

        healthSlider.maxValue = (float)currentUnit.UnitMaxHealth;
        healthSlider.value = (float)currentUnit.UnitHealth;
        healthText.SetText((((float)currentUnit.UnitHealth / (float)currentUnit.UnitMaxHealth) * 100) + "%");
    }

    public void Init(Unit unit)
    {
        currentUnit = unit;
        print(currentUnit);
        Setup();
    }
}
