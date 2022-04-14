using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Utils : MonoBehaviour
{
    public static GenerationAttributes biomeGenerationAttributes = new GenerationAttributes(0.45f, 0.002f, 6, 0.7f);
    public static GenerationAttributes treeGenerationAttributes = new GenerationAttributes(0.10f, 0.9f, 1, 0.7f);
    public static GenerationAttributes cactusGenerationAttributes = new GenerationAttributes(0.15f, 0.9f, 1, 0.7f);

    public static int TerrainHeight(float x, float z, BiomeGenerationInfo info)
    {
        if (info.strength == 1)
        {
            return TerrainHeightOfBiome(x, z, info.biome);
        }
        else
        {
            return (int)(TerrainHeightOfBiome(x, z, info.bottomBiome) * info.strength + TerrainHeightOfBiome(x, z, info.bottomBiome) * (1 - info.strength));
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

    public static BiomeGenerationInfo WhichBiome(float x, float z)
    {
        float bp = FBM(x * biomeGenerationAttributes.smoothness, z * biomeGenerationAttributes.smoothness, biomeGenerationAttributes.octaves, biomeGenerationAttributes.persistence);
        Biome b = (bp < 0.5f) ? Biome.DESERT : Biome.MOUNTAINS;
        if (bp >= 0.45 && bp <= 0.55)
        {
            return new BiomeGenerationInfo(b, Biome.DESERT, Biome.MOUNTAINS, (bp - 0.45f) * 10);
        }
        else
        {
            return new BiomeGenerationInfo(b, Biome.DESERT, Biome.MOUNTAINS, 1f);
        }
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

    public float biomeSmoothness = 0.7f;
    public int biomeOctaves = 1;
    public float biomePersistence = 0.2f;

    private void Start()
    {
        t = 0;
    }

    private void Update()
    {
        /*t += Time.deltaTime;
        float p = TerrainHeightGenerate(t, 1, new TerrainGenerationAttributes(plainsMinHeight, plainsMaxHeight, plainsSmoothness, plainsOctaves, plainsPersistence));
        float m = TerrainHeightGenerate(t, 1, new TerrainGenerationAttributes(mountainsMinHeight, mountainsMaxHeight, mountainsSmoothness, mountainsOctaves, mountainsPersistence));
        float b = Mathf.PerlinNoise(t * biomeSmoothness, (t + WorldGeneration.SEED) * biomeSmoothness);

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
            float new_b = (float)Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0.4f, 0.6f, b));
            h = (new_b * m + (1 - new_b) * p);
        }

        Grapher.Log(p, "Plains", Color.green);
        Grapher.Log(m, "Mountains", Color.black);
        Grapher.Log(b, "Which Biome", Color.blue);
        Grapher.Log(h, "Height", Color.red);*/
    }
}
