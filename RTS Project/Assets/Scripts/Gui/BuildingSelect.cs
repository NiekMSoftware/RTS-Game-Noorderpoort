using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelect : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private TMP_Text buildingName;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject workerInfo;

    private BuildingBase currentBuilding;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));

        workerInfo.SetActive(false);
    }

    private void OnEnable()
    {
        Setup();
    }

    private void Setup()
    {
        if (currentBuilding == null) return;

        buildingName.SetText(currentBuilding.buildingName);

        destroyButton.onClick.RemoveAllListeners();
        destroyButton.onClick.AddListener(currentBuilding.DestroyBuilding);

        if (currentBuilding is ResourceBuildingBase)
        {
            print("is resource building");
            workerInfo.SetActive(true);
        }
        else
        {
            print("is no resource building");
            workerInfo.SetActive(false);
        }
    }

    private void Update()
    {
        healthSlider.value = currentBuilding.buildingHp;
        healthText.SetText(((currentBuilding.buildingHp / 50) * 100) + "%");
    }

    public void SetBuilding(BuildingBase building)
    {
        currentBuilding = building;
        Setup();
    }
}
