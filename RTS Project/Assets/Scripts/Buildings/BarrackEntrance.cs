using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrackEntrance : MonoBehaviour
{
    private Barrack barrack;

    public void Setup(Barrack barrack)
    {
        this.barrack = barrack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (barrack.GetCurrentState() != BuildingBase.States.Normal) return;

        if (other.CompareTag("AI"))
        {
            if (other.TryGetComponent(out Worker worker))
            {
                barrack.AIEnteredEntrance(worker);
            }
        }
    }
}
