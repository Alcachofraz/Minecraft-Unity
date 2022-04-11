using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Utils : MonoBehaviour
{
    public static int PerlinNoise2D(float x, float z, TerrainGenerationAttributes attributes)
    {
        return (int)Map(attributes.minHeight, attributes.maxHeight, 0, 1, FBM(x * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence));
    }

    public static float PerlinNoise3D(float x, float y, float z, GenerationAttributes attributes)
    {
        return FBM3D(x * attributes.smoothness, y * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence);
    }
    public static float Probability2D(float x, float z, GenerationAttributes attributes)
    {
        return FBM2D(x * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence);
    }

    static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
    }

    static float FBM3D(float x, float y, float z, int octaves, float persistence)
    {
        float xy = FBM(x, y, octaves, persistence);
        float yx = FBM(y, x, octaves, persistence);
        float xz = FBM(x, z, octaves, persistence);
        float zx = FBM(z, x, octaves, persistence);
        float yz = FBM(y, z, octaves, persistence);
        float zy = FBM(z, y, octaves, persistence);

        return (xy + yx + xz + zx + yz + zy) / 6;
    }

    static float FBM2D(float x, float z, int octaves, float persistence)
    {
        float xz = FBM(x, z, octaves, persistence);
        float zx = FBM(z, x, octaves, persistence);

        return (xz + zx) / 2;
    }


    static float FBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            
            total += Mathf.PerlinNoise((x + WorldGeneration.SEED) * frequency, (z + WorldGeneration.SEED) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }
        return total / maxValue;
    }

    float t;
    public int plainsMinHeight = 60;
    public int plainsMaxHeight = 80;
    public float plainsSmoothness = 0.2f;
    public int plainsOctaves = 2;
    public float plainsPersistence = 0.7f;
    public int mountainsMinHeight = 70;
    public int mountainsMaxHeight = 120;
    public float mountainsSmoothness = 0.5f;
    public int mountainsOctaves = 4;
    public float mountainsPersistence = 0.7f;
    public int biomeOctaves = 1;
    public float biomePersistence = 0.7f;

    private void Start()
    {
        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;
        float p = PerlinNoise2D(t, 1, new TerrainGenerationAttributes(plainsMinHeight, plainsMaxHeight, plainsSmoothness, plainsOctaves, plainsPersistence));
        float m = PerlinNoise2D(t, 1, new TerrainGenerationAttributes(mountainsMinHeight, mountainsMaxHeight, mountainsSmoothness, mountainsOctaves, mountainsPersistence));
        float b = FBM(t * 0.2f, 1, biomeOctaves, biomePersistence);

        float h;

        if (b < 0.4)
        {
            h = p;
        }
        else if (b > 0.6)
        {
            h = m;
        }
        else
        {
            float new_b = (float) Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0.4f, 0.6f, b));
            h = (new_b * m + (1 - new_b) * p);
        }

        Grapher.Log(p, "Plains", Color.green);
        Grapher.Log(m, "Mountains", Color.black);
        Grapher.Log(b, "Which Biome", Color.blue);
        Grapher.Log(h, "Height", Color.red);
    }
}
