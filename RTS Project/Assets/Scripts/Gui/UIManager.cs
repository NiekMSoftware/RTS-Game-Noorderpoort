using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] resourceTexts;
    [SerializeField] private ResourceItemManager playerResourceItemManager;

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
}