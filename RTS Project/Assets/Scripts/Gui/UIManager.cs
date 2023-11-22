using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] resourceTexts;
    [SerializeField] private ResourceItemManager playerResourceItemManager;
    [SerializeField] private GameObject buildingSelectPanel;

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

    public void SetBuildingSelectPanel(bool value) => buildingSelectPanel.SetActive(value);
}