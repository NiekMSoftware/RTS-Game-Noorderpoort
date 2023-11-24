using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] resourceTexts;
    [SerializeField] private ResourceItemManager playerResourceItemManager;
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
            int amount = playerResourceItemManager.itemSlots[i].amount;
            resourceTexts[i].text = amount.ToString();

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