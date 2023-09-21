using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tile[,] grid;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector3 gridOffset;
    [SerializeField] private LayerMask buildingLayer;

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
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                Tile tile = new();
                grid[x, z] = tile;
                grid[x, z].pos = new Vector3Int(x, 0, z) + gridOffset;
            }
        }
    }

    public Vector3Int GetClosestPointOnGrid(Vector3 pos)
    {
        List<Vector3Int> positions = new();
        List<Tile> tiles = new();
        Vector3Int finalPos;
        float shortestDistance = 100;
        int index = 0;
        Tile finalTile = null;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                tiles.Add(grid[x, z]);
                positions.Add(Vector3Int.FloorToInt(grid[x, z].pos));
            }
        }

        for (int i = 0; i < positions.Count; i++)
        {
            if (Vector3.Distance(positions[i], pos) < shortestDistance)
            {
                index = i;
                shortestDistance = Vector3.Distance(positions[i], pos);
                finalTile = tiles[i];
            }
        }

        if (!finalTile.isOccupied)
        {
            finalPos = positions[index];
        }
        else
        {
            Debug.LogError("Tried to spawn on occupied position : " + finalTile.pos);
            finalPos = Vector3Int.CeilToInt(pos);
        }

        Invoke(nameof(CheckOccupancy), 0.1f);

        return finalPos;
    }

    public void PlaceObject(Vector3 pos)
    {

    }

    private void CheckOccupancy()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                Collider[] colliders = new Collider[1];
                Physics.OverlapSphereNonAlloc(grid[x, z].pos, 0.1f, colliders, buildingLayer);

                if (colliders[0] != null)
                {
                    grid[x, z].isOccupied = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                Gizmos.color = Color.white;

                if (grid[x, z].isOccupied)
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireCube(new Vector3(grid[x, z].pos.x, 0, grid[x, z].pos.z), new Vector3(0.9f, 0, 0.9f));
            }
        }
    }
}