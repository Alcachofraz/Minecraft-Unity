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
    public int strengthMinHeight = 60;
    public int strengthMaxHeight = 140;
    public float strengthSmoothness = 0.05f;
    public int strengthOctaves = 1;
    public float strengthPersistence = 0.7f;

    private void Start()
    {
        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;
        float p = PerlinNoise2D(t, 1, new TerrainGenerationAttributes(plainsMinHeight, plainsMaxHeight, plainsSmoothness, plainsOctaves, plainsPersistence));
        float m = PerlinNoise2D(t, 1, new TerrainGenerationAttributes(mountainsMinHeight, mountainsMaxHeight, mountainsSmoothness, mountainsOctaves, mountainsPersistence));
        float s = FBM(t * strengthSmoothness, 1, strengthOctaves, strengthPersistence); 
        float h;
        float b = FBM(t * 0.2f, 1, strengthOctaves, strengthPersistence);
        if (b < 0.4)
        {
            h = p * s;
        }
        else if (b > 0.6)
        {
            h =  ConvertRange(plainsMinHeight, plainsMaxHeight, mountainsMinHeight, mountainsMaxHeight, p);
        }
        else
        {
            h = (p * s) + (m + s);
        }
        h /= 10;
        Grapher.Log(p, "Plains", Color.green);
        Grapher.Log(m, "Mountains", Color.black);
        Grapher.Log(s, "Strength", Color.magenta);
        Grapher.Log(b, "Which Biome", Color.blue);
        Grapher.Log(h, "Height", Color.red);
    }

    /// <summary>
    /// Converts the range.
    /// </summary>
    /// <param name="originalStart">The original start.</param>
    /// <param name="originalEnd">The original end.</param>
    /// <param name="newStart">The new start.</param>
    /// <param name="newEnd">The new end.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static float ConvertRange(
        float originalStart, float originalEnd, // original range
        float newStart, float newEnd, // desired range
        float value) // value to convert
    {
        float scale = (newEnd - newStart) / (originalEnd - originalStart);
        return (newStart + ((value - originalStart) * scale));
    }
    /// <summary>
    /// Gets the bounded noise.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="meanHeight">Height of the mean.</param>
    /// <param name="amplitude">The amplitude.</param>
    /// <returns></returns>
    // [InRange(-.5f, .5f)] && [InRange(0, 1)]
    public static float GetBoundedNoise(float value, float meanHeight, float amplitude)
    {
        return Mathf.Clamp01(ConvertRange(0, 1, -amplitude, amplitude, ConvertRange(-1, 1, 0, 1, value)) + (meanHeight + .5f));
    }
}
