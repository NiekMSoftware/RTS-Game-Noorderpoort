using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    float distance;
    Camera mainCamera;
    GridManager gridManager;

    public Vector3 pos;
    public bool isOccupied;

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

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

    public void Init(Camera camera, float distance, GridManager gridManager)
    {
        mainCamera = camera;
        this.distance = distance;
        this.gridManager = gridManager;
    }

    public void UpdateLOD()
    {
        //meshRenderer.enabled = Vector3.Distance(transform.position, mainCamera.transform.position) < distance;
        //print(meshRenderer.enabled);
    }
}