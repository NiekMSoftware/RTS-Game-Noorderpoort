using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private MeshFilter terrain;
    [SerializeField] private MeshRenderer terrainRenderer;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float scale;

    private Mesh mesh;

    private void Awake()
    {
        mesh = terrain.mesh;

        Color[] colorMap = new Color[width * height];
        Texture2D texture = new(width, height);
        terrainRenderer.transform.localScale = new Vector3(width, 1, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;
                float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, perlinNoise);
                //mesh.vertices[y * width + x] = new Vector3(x, /*colorMap[y * width + x].grayscale*/0, y);
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        terrainRenderer.sharedMaterial.mainTexture = texture;
    }
}
