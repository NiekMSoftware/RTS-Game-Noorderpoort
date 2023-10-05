using UnityEngine;

public class ProceduralMeshTest : MonoBehaviour
{
    private void Start()
    {
        Mesh mesh = new();
        mesh.name = "Test";

        mesh.vertices = new Vector3[] {
            Vector3.zero, Vector3.right, Vector3.up,
            new Vector3(1.1f, 0f), new Vector3(0f, 1.1f), new Vector3(1.1f, 1.1f)
        };

        mesh.normals = new Vector3[] {
            Vector3.back, Vector3.back, Vector3.back,
            Vector3.back, Vector3.back, Vector3.back
        };

        mesh.triangles = new int[] {
            0, 2, 1
        };

        GetComponent<MeshFilter>().mesh = mesh;
    }
}