using System.Collections.Generic;
using UnityEngine;

public class TileGroupManager : MonoBehaviour
{
    private List<Tile> tiles = new();
    private Camera mainCamera;
    bool shouldUpdate;

    private void Awake()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(UpdateTiles), 0, 1);
    }

    public void AddTile(Tile tile)
    {
        tiles.Add(tile);
        tile.UpdateLOD();
    }

    public void UpdateTiles()
    {
        if (Vector3.Distance(mainCamera.transform.position, transform.position) < 30)
        {
            foreach (Tile tile in tiles)
            {
                tile.UpdateLOD();
            }
        }
    }
}