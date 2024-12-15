using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CellularNoise))]
public class CellularNoiseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CellularNoise terrainGenerator = (CellularNoise)target;
        if (GUILayout.Button("Generate Terrain"))
        {
            terrainGenerator.GenerateTerrain();
        }
    }
}