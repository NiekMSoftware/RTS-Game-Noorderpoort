using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    private void Awake()
    {
        Instance = this;

        //Invoke(nameof(SetupGrid), 1);
        SetupGrid();
    }

    private void SetupGrid()
    {
        Stopwatch stopWatch = Stopwatch.StartNew();

        grid = new Tile[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                grid[x, z] = SpawnTile(new Vector3(x, 0, z) + gridOffset);
            }
        }

        stopWatch.Stop();
        print("Setupgrid : " + stopWatch.ElapsedMilliseconds + "ms");
    }

    //public Vector3Int GetClosestPointOnGrid(Vector3 pos)
    //{
    //    print("get");
    //    float shortestDistance = 100;
    //    int index = 0;

    //    Tile tile = null;

    //    if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, gridLayer))
    //    {
    //        tile = hit.transform.GetComponent<Tile>();
    //    }

    //    //for (int i = 0; i < tilePositions.Count; i++)
    //    //{
    //    //    if (Vector3.Distance(tilePositions[i], pos) < shortestDistance)
    //    //    {
    //    //        index = i;
    //    //        shortestDistance = Vector3.Distance(tilePositions[i], pos);
    //    //    }
    //    //}

    //    return Vector3Int.CeilToInt(tile.pos);
    //}

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

    public void CheckOccupancy(Vector3 pos)
    {
        UnityEngine.Debug.DrawRay(pos, Vector3.down * 100, Color.red, 60f);

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            int amountCollided = 0;

            Tile startTile = hit.transform.GetComponent<Tile>();

            //Collider[] colliders = new Collider[1];
            Collider[] colliders = Physics.OverlapSphere(startTile.pos, 1f, buildingLayer);

            print(colliders[0]);

            if (colliders[0] != null)
            {
                print("First tile collided with building");
                amountCollided++;
                startTile.isOccupied = true;
            }
            else
            {
                print("First tile not occupied");
                startTile.isOccupied = false;
            }

            if (amountCollided > 0)
            {
                amountCollided = 0;

                foreach (var tile in hit.transform.GetComponent<Tile>().GetNeighbours())
                {
                    Collider[] colliders2 = new Collider[1];
                    Physics.OverlapSphereNonAlloc(tile.pos, 0.1f, colliders2, buildingLayer);

                    if (colliders2[0] != null)
                    {
                        print("neighbouring tile : " + tile + " collided with building");
                        amountCollided++;
                        tile.isOccupied = true;
                    }
                    else
                    {
                        tile.isOccupied = false;
                    }
                }
            }
        }

        //for (int x = 0; x < grid.GetLength(0); x++)
        //{
        //    for (int z = 0; z < grid.GetLength(1); z++)
        //    {
        //        Collider[] colliders = new Collider[1];
        //        Physics.OverlapSphereNonAlloc(grid[x, z].pos, 0.1f, colliders, buildingLayer);

        //        if (colliders[0] != null)
        //        {
        //            grid[x, z].isOccupied = true;
        //        }
        //        else
        //        {
        //            grid[x, z].isOccupied = false;
        //        }
        //    }
        //}
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
                    Tile tile = new(this)
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

    private IEnumerator DrawGrid()
    {
        if (grid == null) yield return null;

        Stopwatch stopWatch = Stopwatch.StartNew();

        int count = 100;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                SpawnTile(grid[x, z].pos);
                count--;
                if (count < 0)
                {
                    count = 100;
                    yield return null;
                }
            }
        }

        stopWatch.Stop();
        print("Draw grid : " + stopWatch.ElapsedMilliseconds + "ms");

        yield return null;
    }

    private Tile SpawnTile(Vector3 pos)
    {
        return Instantiate(tilePrefab, pos, Quaternion.identity, transform).GetComponent<Tile>();
    }
}