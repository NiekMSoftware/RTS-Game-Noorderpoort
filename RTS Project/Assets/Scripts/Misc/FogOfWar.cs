using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    // TODO: Refactor the cast performing so it will shoot a ray, then do an overlap sphere on said ground to check the radius
    // if there are things in that radius show them, else not (this goes for all units).

    // TODO: Rewrite the entire fog of war :(

    [Header("Unit Properties")]
    public List<SoldierUnit> soldiers;
    public List<Unit> enemyUnits;

    [Header("Fog of War Properties")]
    [SerializeField] private Transform fogTransform;
}
