using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tile[,] grid;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector3 gridOffset;
    [SerializeField] private LayerMask buildingLayer;
    [SerializeField] private LayerMask tempBuildingLayer;
    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int groupEvery;
    [SerializeField] private TileGroupManager tileGroupManager;

    private void Awake()
    {
        Instance = this;

        SetupGrid();
        DrawGrid();
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
        print("get");
        float shortestDistance = 100;
        int index = 0;

        Tile tile = null;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            tile = hit.transform.GetComponent<Tile>();
        }

        //for (int i = 0; i < tilePositions.Count; i++)
        //{
        //    if (Vector3.Distance(tilePositions[i], pos) < shortestDistance)
        //    {
        //        index = i;
        //        shortestDistance = Vector3.Distance(tilePositions[i], pos);
        //    }
        //}

        return Vector3Int.CeilToInt(tile.pos);
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

    private void DrawGrid()
    {
        if (grid == null) return;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                //add multithreading
                StartCoroutine(SpawnTile(grid[x, z].pos));
            }
        }
    }

    IEnumerator SpawnTile(Vector3 pos)
    {
        Tile spawnedTile = Instantiate(tilePrefab, pos, Quaternion.identity, transform).GetComponent<Tile>();
        spawnedTile.Init(Camera.main, 30, this);

        yield return null;
    }
}