using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    GridManager gridManager;

    public Vector3 pos;
    public bool isOccupied;

    public Tile[] GetNeighbours()
    {
        List<Tile> neighbours = new();

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                if (gridManager.grid[(int)pos.x + x, (int)pos.z + z] != this)
                {
                    neighbours.Add(gridManager.grid[(int)pos.x + x, (int)pos.z + z]);
                }
            }
        }

        return neighbours.ToArray();
    }

    public void Init(Vector3 pos, GridManager gridManager)
    {
        this.pos = pos;
        this.gridManager = gridManager;
    }
}