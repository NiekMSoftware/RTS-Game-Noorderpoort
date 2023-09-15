using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tile[,] grid;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 gridOffset;

    private void Awake()
    {
        Instance = this;

        SetupGrid();
    }

    public class Tile
    {
        public Vector3 pos;
        public bool isOccupied;
    }

    [ContextMenu("Setup Grid")]
    private void SetupGrid()
    {
        grid = new Tile[gridSize.x, gridSize.y];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Tile tile = new();
                grid[x, y] = tile;
                grid[x, y].pos = new Vector2(x, y) + gridOffset;
            }
        }
    }

    public Vector3 GetClosestPointOnGrid(Vector3 pos)
    {
        List<Vector3> positions = new List<Vector3>();
        List<float> distances = new List<float>();
        Vector3 finalPos;
        float shortestDistance = 100;
        int index = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                positions.Add(grid[x, y].pos);
                //positions[x * y] = grid[x, y].pos;
                distances.Add(Vector3.Distance(positions[x * y], pos));
                //distances[x * y] = Vector3.Distance(positions[x * y], pos);

                //print("index : " + (x * y) + " Distance : " + distances[x * y] + " min distance : " + Mathf.Min(distances));

                //print(distances.Min());

                if (distances.Min() < shortestDistance)
                {
                    index = x * y;
                    shortestDistance = distances.Min();
                    //print(shortestDistance);
                }
            }
        }

        finalPos = positions[index];

        return finalPos;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Gizmos.DrawWireCube(new Vector3(grid[x, y].pos.x, 0, grid[x, y].pos.y), new Vector3(1, 0, 1));
            }
        }
    }
}