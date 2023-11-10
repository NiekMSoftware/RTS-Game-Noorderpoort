using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] resourceTexts;
    private ResourceItemManager resourceItemManager;

    private void Start()
    {
        resourceItemManager = GameObject.Find("resourceManager").GetComponent<ResourceItemManager>();
    }

    private void Update()
    {
        for (int i = 0; i < resourceTexts.Length; i++)
        {
            int amount = resourceItemManager.itemSlots[i].amount;
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