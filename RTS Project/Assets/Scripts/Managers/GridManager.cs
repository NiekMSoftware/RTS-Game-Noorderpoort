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
        public Vector2 pos;
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
        Vector3[] positions = new Vector3[grid.Length];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                positions[x * y] = grid[x, y].pos;
                print(positions[x * y]);
            }
        }

        return Vector3.zero;
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