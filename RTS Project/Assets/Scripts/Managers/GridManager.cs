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

    private List<Vector3> tilePos = new();

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
                Vector3 pos = new Vector3(x, 0, z) + gridOffset;
                grid[x, z] = SpawnTile(pos);
                grid[x, z].Init(pos, this);
            }
        }

        stopWatch.Stop();
        print("Setupgrid : " + stopWatch.ElapsedMilliseconds + "ms");
    }

    public bool GetOccupany(Vector3 pos, GameObject building)
    {
        building.layer = (int)Mathf.Log(tempBuildingLayer.value, 2);

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            Tile startTile = hit.transform.GetComponent<Tile>();
            tilePos.Add(startTile.pos);

            Collider[] colliders = new Collider[1];
            Physics.OverlapSphereNonAlloc(startTile.pos, 0.1f, colliders, buildingLayer);

            if (colliders[0] != null)
            {
                print("nooo!");
                building.layer = (int)Mathf.Log(buildingLayer.value, 2);
                return true;
            }

            List<Tile> tiles = new();
            List<Tile> tempTiles = new();
            tiles.Add(startTile);

            for (int i = 0; i < 3; i++)
            {
                for (int e = 0; e < tiles.Count; e++)
                {
                    foreach (var tile in tiles[e].GetNeighbours())
                    {
                        Collider[] colliders2 = new Collider[1];
                        Physics.OverlapSphereNonAlloc(tile.pos, 0.1f, colliders2, buildingLayer);

                        if (colliders2[0] != null)
                        {
                            building.layer = (int)Mathf.Log(buildingLayer.value, 2);
                            return true;
                        }
                    }
                }


                tiles.Clear();
                tiles = new List<Tile>(tempTiles);
                tempTiles.Clear();
            }

            building.layer = (int)Mathf.Log(buildingLayer.value, 2);
            return false;
        }

        building.layer = (int)Mathf.Log(buildingLayer.value, 2);
        return false;
    }

    public void SetOccupancy(Vector3 pos)
    {
        UnityEngine.Debug.DrawRay(pos, Vector3.down * 100, Color.red, 60f);

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            Tile startTile = hit.transform.GetComponent<Tile>();
            tilePos.Add(startTile.pos);

            Collider[] colliders = new Collider[1];
            Physics.OverlapSphereNonAlloc(startTile.pos, 0.1f, colliders, buildingLayer);

            if (colliders[0] != null)
            {
                startTile.isOccupied = true;
                startTile.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else
            {
                startTile.isOccupied = false;
                startTile.GetComponent<MeshRenderer>().material.color = Color.white;
            }

            List<Tile> tiles = new();
            List<Tile> tempTiles = new();
            tiles.Add(startTile);

            for (int i = 0; i < 3; i++)
            {
                for (int e = 0; e < tiles.Count; e++)
                {
                    foreach (var tile in tiles[e].GetNeighbours())
                    {
                        Collider[] colliders2 = new Collider[1];
                        Physics.OverlapSphereNonAlloc(tile.pos, 0.1f, colliders2, buildingLayer);

                        if (colliders2[0] != null)
                        {
                            tile.isOccupied = true;
                            tile.GetComponent<MeshRenderer>().material.color = Color.red;
                            tempTiles.Add(tile);
                        }
                        else
                        {
                            tile.isOccupied = false;
                            tile.GetComponent<MeshRenderer>().material.color = Color.white;
                        }
                    }
                }


                tiles.Clear();
                tiles = new List<Tile>(tempTiles);
                tempTiles.Clear();
            }
        }
    }

    private Tile SpawnTile(Vector3 pos)
    {
        return Instantiate(tilePrefab, pos, Quaternion.identity, transform).GetComponent<Tile>();
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 pos in tilePos)
        {
            Gizmos.DrawWireSphere(pos, 0.1f);
        }
    }
}