using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private List<Unit> units = new();

    private void Update()
    {
        GetUnits();
    }

    private void GetUnits()
    {
        Unit temp = FindObjectOfType<Unit>();

        if (units.Count == 0)
        {
            units.Add(temp);
        }
    }
}
