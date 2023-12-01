using TMPro;
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

    private NewSelectionManager selectionManager;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Deselect);

        locationButton.onClick.RemoveAllListeners();
        locationButton.onClick.AddListener(GoToDestination);
    }

    private void GoToDestination()
    {
        mainCamera.transform.position = agent.destination;
    }

    private void Deselect()
    {
        selectionManager.DeselectAllUnits();
        gameObject.SetActive(false);
    }

    private void Setup()
    {
        if (currentUnit == null) return;

        agent = currentUnit.GetComponent<NavMeshAgent>();
        icon.texture = currentUnit.GetRenderTexture();
        nameText.SetText(currentUnit.UnitName);

        if (currentUnit.GetCurrentAction() == null)
        {
            if (agent.destination == currentUnit.transform.position || agent.isStopped == true || agent.velocity == Vector3.zero)
            {
                locationText.SetText("Idling");
            }
            else
            {
                locationText.SetText("Doing something");
            }
        }
        else
        {
            locationText.SetText(currentUnit.GetCurrentAction());
        }
    }

    private void Update()
    {
        if (!currentUnit) return;

        healthSlider.maxValue = (float)currentUnit.UnitMaxHealth;
        healthSlider.value = (float)currentUnit.UnitHealth;
        healthText.SetText((((float)currentUnit.UnitHealth / (float)currentUnit.UnitMaxHealth) * 100) + "%");
    }

    public void Init(Unit unit, NewSelectionManager selectionManager)
    {
        this.selectionManager = selectionManager;
        currentUnit = unit;
        Setup();
    }
}
