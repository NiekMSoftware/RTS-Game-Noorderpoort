using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelect : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button assignWorkersButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private TMP_Text buildingName;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject workerInfo;
    [SerializeField] private Transform workerInfoParent;
    [SerializeField] private GameObject singleWorkerInfoPrefab;

    private NewSelectionManager selectionManager;

    private BuildingBase currentBuilding;
    
    private int maxBuildingHealth = 50;

    private List<GameObject> singleWorkerInfoUIs = new();

    private bool isSelectingWorkers;

    private void Awake()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));

        workerInfo.SetActive(false);

        selectionManager = FindObjectOfType<NewSelectionManager>();
    }

    private void OnEnable()
    {
        Setup();
    }

    public void Setup()
    {
        if (!currentBuilding) return;

        buildingName.SetText(currentBuilding.buildingName);

        destroyButton.onClick.RemoveAllListeners();
        destroyButton.onClick.AddListener(currentBuilding.DestroyBuilding);

        if (currentBuilding is ResourceBuildingBase resourceBuilding)
        {
            assignWorkersButton.gameObject.SetActive(true);

            assignWorkersButton.onClick.RemoveAllListeners();
            assignWorkersButton.onClick.AddListener(ToggleAssignWorkers);

            if (singleWorkerInfoUIs.Count > 0)
            {
                for (int i = 0; i < singleWorkerInfoUIs.Count; ++i)
                {
                    Destroy(singleWorkerInfoUIs[i]);
                }

                singleWorkerInfoUIs.Clear();
            }

            for (int i = 0; i < resourceBuilding.GetWorkers().Count; i++)
            {
                WorkerUI singleWorkerInfo = Instantiate(singleWorkerInfoPrefab, workerInfoParent).GetComponent<WorkerUI>();
                Worker worker = resourceBuilding.GetWorkers()[i];
                singleWorkerInfo.Setup(worker.name, worker, this);
                singleWorkerInfoUIs.Add(singleWorkerInfo.gameObject);
            }

            workerInfo.SetActive(true);
        }
        else
        {
            assignWorkersButton.gameObject.SetActive(false);

            workerInfo.SetActive(false);
        }
    }

    private void Update()
    {
        healthSlider.value = currentBuilding.buildingHp;
        healthText.SetText(((currentBuilding.buildingHp / maxBuildingHealth) * 100) + "%");

        if (currentBuilding is ResourceBuildingBase)
        {
            TMP_Text assignWorkerButtonText = assignWorkersButton.GetComponentInChildren<TMP_Text>();
            assignWorkerButtonText.SetText(isSelectingWorkers ? "Done" : "Assign Workers"); 
        }
    }

    private void ToggleAssignWorkers()
    {
        if (isSelectingWorkers && currentBuilding is ResourceBuildingBase workerBuilding)
        {
            foreach (var unit in selectionManager.GetSelectedUnits())
            {
                if (unit is Worker worker)
                {
                    if (workerBuilding.AddWorkerToBuilding(worker))
                    {
                        worker.Select();
                        Setup();
                    }
                }
            }
        }

        selectionManager.DeselectAllUnits();

        isSelectingWorkers = !isSelectingWorkers;
    }

    public void SetBuilding(BuildingBase building)
    {
        currentBuilding = building;
        Setup();
    }

    public bool GetIsSelecting() => isSelectingWorkers;
}
