using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSoldierSelect : MonoBehaviour
{
    [SerializeField] private Button drawFormationButton;
    [SerializeField] private GameObject soldierSplineMove;
    [SerializeField] private NewSelectionManager selectionManager;

    private void Start()
    {
        drawFormationButton.onClick.RemoveAllListeners();
        drawFormationButton.onClick.AddListener(EnableSoldierSplineMove);
    }

    private void EnableSoldierSplineMove()
    {
        selectionManager.SetMayDeselect(false);
        selectionManager.SetMayDrawSelectionBox(false);
        soldierSplineMove.SetActive(true);
    }
}
