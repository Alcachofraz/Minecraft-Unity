using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int GenerateHeight(float x, float z, float maxHeight, float smooth, int octaves, float persistence)
    {
        return (int)Map(0, maxHeight, 0, 1, FBM(x * smooth, z * smooth, octaves, persistence));
    }

    static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
    }

    static float FBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            amplitude *= persistence;
            frequency *= 2;
            maxValue += amplitude;
        }
        return total / maxValue;
    }
}
