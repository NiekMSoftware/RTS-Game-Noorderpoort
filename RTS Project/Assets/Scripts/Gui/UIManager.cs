using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI woodResourceNumber;
    public TextMeshProUGUI stoneResourceNumber;
    public TextMeshProUGUI metalResourceNumber;

    private ResourceItemManager resourceItemManager;

    private void Start()
    {   
        resourceItemManager = GameObject.Find("ResourceManager").GetComponent<ResourceItemManager>();
    }

    private void Update()
    {
        woodResourceNumber.text = resourceItemManager.itemSlots[0].amount.ToString();
        stoneResourceNumber.text = resourceItemManager.itemSlots[1].amount.ToString();
        metalResourceNumber.text = resourceItemManager.itemSlots[2].amount.ToString();

        if (resourceItemManager.itemSlots[0].amount == 0)
        {
            woodResourceNumber.color = Color.red;
        }
        else
        {
            woodResourceNumber.color = Color.white;
        }



    }
}
