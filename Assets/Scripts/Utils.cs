using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Utils : MonoBehaviour
{
    public static int PerlinNoise2D(float x, float z, FloorGenerationAttributes attributes)
    {
        return (int)Map(attributes.minHeight, attributes.maxHeight, 0, 1, FBM(x * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence));
    }

    public static float PerlinNoise3D(float x, float y, float z, GenerationAttributes attributes)
    {
        return FBM3D(x * attributes.smoothness, y * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence);
    }
    public static float BiomeProbability(float x, float z, GenerationAttributes attributes)
    {
        return FBM2D(x * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence);
    }

    public static float CactusProbability(float x, float z, GenerationAttributes attributes)
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
    public float smoothness = 0.5f;
    public int octaves = 6;
    public float persistence = 0.7f;

    private void Start()
    {
        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;
        float n = PerlinNoise3D(t, t * 2, t * 3, new GenerationAttributes(0.045f, 0.02f, 6, 0.7f));
        Grapher.Log(n, "Noise 3D", Color.red);
    }
}
