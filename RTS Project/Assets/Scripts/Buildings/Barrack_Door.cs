using UnityEngine;

public class Barrack_Door : MonoBehaviour
{
    private Barrack barrack;

    public void SetBarrack(Barrack barrack)
    {
        this.barrack = barrack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AI"))
        {
            // Destroy the unit
            Destroy(other.gameObject);

            // Add a new item to the list!
            barrack.UnitList.Add(barrack.unitToSpawn);
        }
    }
}
