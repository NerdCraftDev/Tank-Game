using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TerrainData))]
public class TerrainGenerator : MonoBehaviour
{
    public TerrainData terrainData;
    public float terrainHeight = 100f;
    public Vector3 terrainSize;
    public Vector3 terrainScale;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public NoiseType noiseType = NoiseType.Perlin;
        public float strength = 1f;
        public float frequency = 1f;
        public float octaves = 2f;
        public float persistence = 0.5f;
        public float horizontalScale = 1;
        public Vector3 offset;
        public bool useMask = false;
        public MaskLayer maskLayer;
    }

    [System.Serializable]
    public class MaskLayer
    {
        public NoiseLayer layer;
    }

    public NoiseLayer[] noiseLayers;

    public void GenerateTerrain()
    {
        terrainData = GetComponent<TerrainCollider>().terrainData;
        terrainData.size = terrainSize;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        float[,] heightmap = new float[heightmapWidth, heightmapHeight];

        for (int x = 0; x < heightmapWidth; x++) {
            for (int y = 0; y < heightmapHeight; y++) {
                float height = 0f;
                foreach (NoiseLayer layer in noiseLayers) {
                    if (layer.enabled) {
                        float maskValue = 1f;
                        if (layer.useMask && layer.maskLayer != null) {
                            if (layer.maskLayer.layer.enabled) {
                                maskValue = GetNoise(layer.maskLayer.layer, x, y, heightmapWidth, heightmapHeight) * layer.maskLayer.layer.strength;
                            }
                        }

                        height += GetNoise(layer, x, y, heightmapWidth, heightmapHeight) * maskValue;
                    }
                }
                heightmap[x, y] = height * terrainHeight / terrainSize.y / noiseLayers.Length;
            }
        }

        terrainData.SetHeights(0, 0, heightmap);
    }

    float GetNoise(NoiseLayer layer, int x, int y, int width, int height)
    {
        float xCoord = (float)x / width * layer.horizontalScale + layer.offset.x;
        float yCoord = (float)y / height * layer.horizontalScale + layer.offset.z;
        
        float frequency = layer.frequency;
        float amplitude = 1f;
        float sum = 0f;

        for (int i = 0; i < layer.octaves; i++)
        {
            float noise = GetNoiseValue(layer.noiseType, xCoord * frequency, yCoord * frequency) * terrainScale.y;
            sum += noise * amplitude;
            amplitude *= layer.persistence;
            frequency *= 2f;
        }

        return sum * layer.strength + layer.offset.y * terrainScale.y;
    }

    float GetNoiseValue(NoiseType noiseType, float x, float y)
    {
        switch (noiseType)
        {
            case NoiseType.Perlin:
                return Mathf.PerlinNoise(x / terrainScale.x, y / terrainScale.z);
            case NoiseType.Simplex:
                return noise.snoise(new Vector2(x / terrainScale.x, y / terrainScale.z));
            case NoiseType.Cellular:
                return noise.cellular(new Vector2(x / terrainScale.x, y / terrainScale.z)).x;
            default:
                return 0f;
        }
    }

    public enum NoiseType
    {
        Perlin,
        Simplex,
        Cellular
    }
}