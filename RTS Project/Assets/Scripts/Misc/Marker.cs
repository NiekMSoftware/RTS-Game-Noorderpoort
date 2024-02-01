using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] private MeshRenderer selectionCircle;
    [SerializeField] private Color color;
    [SerializeField] private float acceptDistance;

    private List<Unit> units = new();

    private void Start()
    {
        selectionCircle.material.color = color;
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    private void Update()
    {
        if (units.Count > 0)
        {
            foreach (var unit in units.ToList())
            {
                if (Vector3.Distance(unit.transform.position, transform.position) < acceptDistance)
                {
                    units.Remove(unit);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}