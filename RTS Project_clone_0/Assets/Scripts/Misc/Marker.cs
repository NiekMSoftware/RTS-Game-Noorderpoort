using System.Collections;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    [SerializeField] private MeshRenderer selectionCircle;
    [SerializeField] private Color color;
    [SerializeField] private float acceptDistance;

    private Unit unit;

    IEnumerator Start()
    {
        selectionCircle.material.color = color;

        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    private void Update()
    {
        if (unit != null)
        {
            if (Vector3.Distance(unit.transform.position, transform.position) < acceptDistance)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}