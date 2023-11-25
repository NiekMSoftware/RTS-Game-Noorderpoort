using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] resourceTexts;
    [SerializeField] private TMP_Text[] resourceTextsToolTip;
    [SerializeField] private ResourceItemManager playerResourceItemManager;
    [SerializeField] private TMP_Text unitsHasHomeText;
    [SerializeField] private TMP_Text unitsNoHomeText;
    [SerializeField] private TMP_Text unitsWorkingText;
    [SerializeField] private TMP_Text unitsWorklessText;
    [SerializeField] private BuildingSelect buildingSelectMenu;
    [SerializeField] private UnitSelect unitSelectMenu;
    [SerializeField] private float outlineDefaultSize;
    [SerializeField] private float outlineAnimationSpeed;
    [SerializeField] private float outlineAnimationMaxSize;
    [SerializeField] private float outlineAnimationFinishedWaitTime;

    private void Start()
    {
        unitSelectMenu.gameObject.SetActive(false);
        buildingSelectMenu.gameObject.SetActive(false);
    }

    private void Update()
    {
        for (int i = 0; i < resourceTexts.Length; i++)
        {
            ItemSlot currentItemSlot = playerResourceItemManager.itemSlots[i];
            int amount = currentItemSlot.amount;
            resourceTexts[i].SetText(amount.ToString());

            int maxAmount = currentItemSlot.maxAmount;
            string name = currentItemSlot.data.name;
            resourceTextsToolTip[i].SetText($"{name} : {amount}/{maxAmount}");

            if (amount == 0)
            {
                resourceTexts[i].color = Color.red;
            }
            else
            {
                resourceTexts[i].color = Color.white;
            }
        }
    }

    public void UpdateWorkerUI()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        int hasHomeAmount = 0;
        int noHomeAmount = 0;
        int workingAmount = 0;
        int worklessAmount = 0;

        for (int i = 0; i < allUnits.Length; i++)
        {
            if (allUnits[i] is Worker worker)
            {
                if (worker.GetHasWork())
                {
                    workingAmount++;
                }
                else
                {
                    worklessAmount++;
                }
            }
        }

        unitsHasHomeText.SetText(hasHomeAmount.ToString());
        unitsNoHomeText.SetText(noHomeAmount.ToString());
        unitsWorkingText.SetText(workingAmount.ToString());
        unitsWorklessText.SetText(worklessAmount.ToString());
    }

    public void SetBuildingUI(bool value, BuildingBase building)
    {
        buildingSelectMenu.SetBuilding(building);
        buildingSelectMenu.gameObject.SetActive(value);
    }

    public void SetUnitUI(bool value, Unit unit)
    {
        unitSelectMenu.gameObject.SetActive(value);
        unitSelectMenu.Init(unit);
    }

    public float GetOutlineAnimationSpeed() => outlineAnimationSpeed;

    public float GetOutlineAnimationMaxSize() => outlineAnimationMaxSize;

    public float GetOutlineAnimationFinishedWaitTime() => outlineAnimationFinishedWaitTime;

    public float GetOutlineDefaultSize() => outlineDefaultSize;
}