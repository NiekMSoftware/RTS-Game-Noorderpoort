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

    //private List<Vector3Int> tilePositions = new();

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

        //int groupsToSpawnX = grid.GetLength(0) / groupEvery;
        //int groupsToSpawnZ = grid.GetLength(1) / groupEvery;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                Tile tile = new();
                grid[x, z] = tile;
                grid[x, z].pos = new Vector3Int(x, 0, z) + gridOffset;
            }
        }

        //for (int x = 0; x < 1; x++)
        //{
        //    for (int z = 0; z < 1; z++)
        //    {
        //        SetupGroup();
        //    }
        //}

        //foreach (var item in grid[5, 7].GetNeighbours())
        //{
        //    item.isOccupied = true;
        //}

        //CheckOccupancy();
    }

    private void SetupGroup()
    {
        for (int x = 0; x < groupEvery; x++)
        {
            for (int z = 0; z < groupEvery; z++)
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

        GameObject currentGroupParentObject;

        currentGroupParentObject = new("Tile Group");
        currentGroupParentObject.AddComponent<TileGroupManager>();
        currentGroupParentObject.transform.parent = transform;

        int i = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                i++;

                if (i == groupEvery)
                {
                    i = 0;
                    currentGroupParentObject = new("Tile Group");
                    currentGroupParentObject.AddComponent<TileGroupManager>();
                    currentGroupParentObject.transform.position = grid[x, z].pos;
                    currentGroupParentObject.transform.parent = transform;
                }

                //add multithreading
                Tile spawnedTile = Instantiate(tilePrefab, grid[x, z].pos, Quaternion.identity, currentGroupParentObject.transform).GetComponent<Tile>();
                spawnedTile.Init(Camera.main, 30, this);
                currentGroupParentObject.GetComponent<TileGroupManager>().AddTile(spawnedTile);
            }
        }

        //foreach (var tile in grid)
        //{
        //    i++;

        //    if (i == groupEvery)
        //    {
        //        i = 0;
        //        currentGroupParentObject = new("Tile Group");
        //        currentGroupParentObject.AddComponent<TileGroupManager>();
        //        currentGroupParentObject.transform.position = tile.pos;
        //        currentGroupParentObject.transform.parent = transform;
        //    }

        //    Tile spawnedTile = Instantiate(tilePrefab, tile.pos, Quaternion.identity, currentGroupParentObject.transform).GetComponent<Tile>();
        //    spawnedTile.Init(Camera.main, 30, this);
        //    currentGroupParentObject.GetComponent<TileGroupManager>().AddTile(spawnedTile);
        //}
    }

    //private void OnDrawGizmos()
    //{
    //    if (grid == null) return;

    //    for (int x = 0; x < grid.GetLength(0); x++)
    //    {
    //        for (int z = 0; z < grid.GetLength(1); z++)
    //        {
    //            Gizmos.color = Color.white;

    //            if (grid[x, z].isOccupied)
    //            {
    //                Gizmos.color = Color.red;
    //            }

    //            Gizmos.DrawWireCube(new Vector3(grid[x, z].pos.x, 0, grid[x, z].pos.z), new Vector3(0.9f, 0, 0.9f));
    //        }
    //    }
    //}
}