using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public static Tile[,] grid;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector3 gridOffset;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private LayerMask tempBuildingLayer;
    [SerializeField] private BuildingManager buildingManager;

    private List<Vector3Int> tilePositions = new();

    private void Awake()
    {
        Instance = this;

        SetupGrid();
    }

    public class Tile
    {
        public Vector3 pos;
        public bool isOccupied;

        public Tile[] GetNeighbours()
        {
            List<Tile> neighbours = new();

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (grid[x, y] != this)
                    {
                        neighbours.Add(grid[x, y]);
                    }
                }
            }

            return neighbours.ToArray();
        }
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
                tilePositions.Add(Vector3Int.FloorToInt(grid[x, z].pos));
            }
        }

        foreach (var item in grid[1, 1].GetNeighbours())
        {
            item.isOccupied = true;
        }

        //CheckOccupancy();
    }

    public Vector3Int GetClosestPointOnGrid(Vector3 pos)
    {
        print("get");
        float shortestDistance = 100;
        int index = 0;

        for (int i = 0; i < tilePositions.Count; i++)
        {
            if (Vector3.Distance(tilePositions[i], pos) < shortestDistance)
            {
                index = i;
                shortestDistance = Vector3.Distance(tilePositions[i], pos);
            }
        }

        return tilePositions[index];
    }

    public bool GetOccupanyPendingObject()
    {
        //Tile[] tiles = CheckOccupancyPendingObject();

        //foreach (var tile in tiles)
        //{
        //    if (tile.isOccupied)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }

    public void CheckOccupancy()
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
                else
                {
                    grid[x, z].isOccupied = false;
                }
            }
        }
    }

    public Tile[] CheckOccupancyPendingObject()
    {
        List<Tile> tiles = new();

        buildingManager.GetPendingObject().layer = (int)Mathf.Log(tempBuildingLayer.value, 2);

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                Collider[] colliders = new Collider[1];
                Physics.OverlapSphereNonAlloc(grid[x, z].pos, 0.1f, colliders, tempBuildingLayer);

                if (colliders[0] != null)
                {
                    Tile tile = new()
                    {
                        pos = grid[x, z].pos,
                        isOccupied = grid[x, z].isOccupied,
                    };

                    tiles.Add(tile);
                }
            }
        }

        buildingManager.GetPendingObject().layer = (int)Mathf.Log(buildingLayer.value, 2);

        return tiles.ToArray();
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