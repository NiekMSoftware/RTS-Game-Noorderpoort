using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

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
			Tile startTile = hit.transform.GetComponent<Tile>();
			tilePos.Add(startTile.pos);

			Collider[] colliders = new Collider[1];
			Physics.OverlapSphereNonAlloc(startTile.pos, 0.1f, colliders, buildingLayer);

			if (colliders[0] != null)
			{
				print("First tile collided with building");
				startTile.isOccupied = true;
				startTile.GetComponent<MeshRenderer>().material.color = Color.red;
			}
			else
			{
				print("First tile not occupied");
				startTile.isOccupied = false;
				startTile.GetComponent<MeshRenderer>().material.color = Color.white;
			}

			List<Tile> tiles = new();
            List<Tile> tempTiles = new();
            tiles.Add(startTile);

			while (tiles.Count > 0)
			{
				print("Tiles count : " + tiles.Count);

                for (int i = 0; i < tiles.Count; i++)
				{
					print("i : " + i);
					foreach (var tile in tiles[i].GetNeighbours())
					{
						print("Neighbouring tile : " + tile);
						Collider[] colliders2 = new Collider[1];
						Physics.OverlapSphereNonAlloc(tile.pos, 0.1f, colliders2, buildingLayer);
						tilePos.Add(tile.pos);

						if (colliders2[0] != null)
						{
							print("neighbouring tile : " + tile + " collided with building");
							tile.isOccupied = true;
							tile.GetComponent<MeshRenderer>().material.color = Color.red;
							tempTiles.Add(tile);
						}
						else
						{
                            print("neighbouring tile : " + tile + " DID NOT collide with building");
                            tile.isOccupied = false;
							tile.GetComponent<MeshRenderer>().material.color = Color.white;
						}
					}

                    tiles.Clear();
                    tiles = new List<Tile>(tempTiles);
                    tempTiles.Clear();
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
					//Tile tile = new(this)
					//{
					//    pos = grid[x, z].pos,
					//    isOccupied = grid[x, z].isOccupied,
					//};

					//tiles.Add(tile);
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

	private void OnDrawGizmos()
	{
		foreach (Vector3 pos in tilePos)
		{
			Gizmos.DrawWireSphere(pos, 0.1f);
		}
	}
}