using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static GenerationAttributes biomeGenerationAttributes = new GenerationAttributes(0.45f, 0.02f, 1, 0.7f);
    public static GenerationAttributes treeGenerationAttributes = new GenerationAttributes(0.04f, 0.9f, 1, 0.7f);
    public static GenerationAttributes cactusGenerationAttributes = new GenerationAttributes(0.12f, 0.9f, 1, 0.7f);
    public static GenerationAttributes caveGenerationAttributes = new GenerationAttributes(0.45f, 0.005f, 8, 0.5f);

    public static int TerrainHeight(float x, float z, BiomeGenerationInfo info)
    {
        if (info.strength == 1)
        {
            return TerrainHeightOfBiome(x, z, info.biome);
        }
        else
        {
            return (int)(TerrainHeightOfBiome(x, z, info.topBiome) * info.strength + TerrainHeightOfBiome(x, z, info.bottomBiome) * (1 - info.strength));
        }
    }

    public static int TerrainHeightOfBiome(float x, float z, Biome b)
    {
        return (int)Map(b.GetFloorGenerationAttributes().minHeight, b.GetFloorGenerationAttributes().maxHeight, 0, 1, FBM(x * b.GetFloorGenerationAttributes().smoothness, z * b.GetFloorGenerationAttributes().smoothness, b.GetFloorGenerationAttributes().octaves, b.GetFloorGenerationAttributes().persistence));
    }

    public static float Noise3D(float x, float y, float z, GenerationAttributes attributes)
    {
        return FBM3D(x * attributes.smoothness, y * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence);
    }

    public static float BiomeRate(float x, float z)
    {
        return FBM(x * biomeGenerationAttributes.smoothness, z * biomeGenerationAttributes.smoothness, biomeGenerationAttributes.octaves, biomeGenerationAttributes.persistence);
    }

    public static bool IsCave(float x, float y, float z)
    {
        float caveProbability = Utils.Noise3D(x, y, z, caveGenerationAttributes);
        return caveProbability > caveGenerationAttributes.probability - 0.01 && caveProbability < caveGenerationAttributes.probability;
    }

    public static bool IsTree(float x, float z)
    {
        return FBM(x * treeGenerationAttributes.smoothness, z * treeGenerationAttributes.smoothness, treeGenerationAttributes.octaves, treeGenerationAttributes.persistence) < treeGenerationAttributes.probability;
    }

    public static bool IsCactus(float x, float z)
    {
        return FBM(x * cactusGenerationAttributes.smoothness, z * cactusGenerationAttributes.smoothness, cactusGenerationAttributes.octaves, cactusGenerationAttributes.persistence) < cactusGenerationAttributes.probability;
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

    static float FBM(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 0.5f;
        float frequency = 1f;
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
}
