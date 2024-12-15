using UnityEngine;
using Unity.Mathematics;

public class CellularNoise : MonoBehaviour
{
    public int width = 256;
    public int length = 256;
    public float cellSize = 10f;
    public float maxHeight = 10f;
    public float edgeHeight = 0.5f;
    public float seed;

    private float[,] heights;

    public void GenerateTerrain()
    {
        GenerateHeights();
        CreateMesh();
    }

    void GenerateHeights()
    {
        heights = new float[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                float value = noise.cellular(new float2((x + seed) / cellSize, (y + seed) / cellSize)).x;
                heights[x, y] = Mathf.Lerp(0, maxHeight, value);
            }
        }
    }

    void CreateMesh()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
        int xMeshCount = Mathf.CeilToInt(width/256f);
        int xExtra = xMeshCount*256-width;
        int yMeshCount = Mathf.CeilToInt(length/256f);
        int yExtra = yMeshCount*256-length;
        for (int a = 0; a < xMeshCount; a++) {
            for (int b = 0; b < yMeshCount; b++) {
                GameObject obj = new GameObject("Mesh " + (a*yMeshCount + b + 1));
                obj.transform.SetParent(transform, false);
                Mesh mesh = new Mesh();
                mesh.name = "Mesh";
                obj.AddComponent<MeshFilter>().mesh = mesh;

                Vector3[] vertices = new Vector3[256 * 256];
                int[] triangles = new int[(256 - 1) * (256 - 1) * 6];
                Color[] colors = new Color[256 * 256];

                int triIndex = 0;
                for (int x = 0; x < 256 - (a == xMeshCount-1 ? xExtra : 0); x++)
                {
                    for (int y = 0; y < 256 - (b == yMeshCount-1 ? yExtra : 0); y++)
                    {
                        float height = heights[x+a*256, y+b*256];
                        vertices[x + y * 256] = new Vector3(x+a*255, height, y+b*255);
                        colors[x + y * 256] = new Color(height / maxHeight, height / maxHeight, height / maxHeight, 1);

                        if (x < 256 - 1 && y < 256 - 1)
                        {
                            triangles[triIndex] = x + y * 256;
                            triangles[triIndex + 1] = x + (y + 1) * 256;
                            triangles[triIndex + 2] = x + 1 + y * 256;
                            triangles[triIndex + 3] = x + 1 + y * 256;
                            triangles[triIndex + 4] = x + (y + 1) * 256;
                            triangles[triIndex + 5] = x + 1 + (y + 1) * 256;
                            triIndex += 6;
                        }
                    }
                }

                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.colors = colors;

                obj.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/VertexColoredMesh"));;

                mesh.RecalculateNormals();
            }
        }
    }
}