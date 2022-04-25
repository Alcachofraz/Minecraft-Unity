using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static GenerationAttributes biomeGenerationAttributes = new GenerationAttributes(0.0f/*not used*/, 0.005f, 1, 0.7f);
    public static GenerationAttributes treeGenerationAttributes = new GenerationAttributes(0.04f, 0.9f, 1, 0.7f);
    public static GenerationAttributes cactusGenerationAttributes = new GenerationAttributes(0.08f, 0.9f, 1, 0.7f);
    public static TerrainGenerationAttributes cactusHeightGenerationAttributes = new TerrainGenerationAttributes(1, 4, 1.2f, 4, 0.5f);

    public static int TerrainHeight(int x, int z)
    {
        BiomeGenerationInfo info = GetGenerationInfo(x, z);
        float ret = 0.0f;
        foreach (KeyValuePair<Biome, (float, float)> strengths in info.strengths)
        {
            float h = TerrainHeightOfBiome(x, z, strengths.Key);
            ret += (h * strengths.Value.Item1 * strengths.Value.Item2);
        }
        return (int)ret;

        /*BiomeGenerationInfo info = GetGenerationInfo(x, z);
        float ret;
        float h_h = 0;
        foreach (KeyValuePair<Biome, (float, float)> strengths in info.strengths)
        {
            float h = TerrainHeightOfBiome(x, z, strengths.Key);
            h_h += h * strengths.Value.Item1;
        }
        float h_t = 0;
        foreach (KeyValuePair<Biome, (float, float)> strengths in info.strengths)
        {
            float h = TerrainHeightOfBiome(x, z, strengths.Key);
            h_t += h * strengths.Value.Item2;
        }
        ret = (h_h + h_t) / 2;
        return (int)ret;*/
    }

    public static Biome WhichBiome(int x, int z)
    {
        Biome[] biomes = (Biome[])System.Enum.GetValues(typeof(Biome));
        foreach (Biome biome in biomes)
        {
            float h = Humidity(x, z);
            if (h > biome.GetWhittakerLimits().MinHumidity && h < biome.GetWhittakerLimits().MaxHumidity)
            {
                float t = Temperature(x, z);
                if (t > biome.GetWhittakerLimits().MinTemperature && h < biome.GetWhittakerLimits().MaxTemperature)
                {
                    return biome;
                }
            }
        }
        return Biome.PLAINS;
    }

    public static int TerrainHeightOfBiome(int x, int z, Biome b)
    {
        return (int)Map(b.GetFloorGenerationAttributes().minHeight, b.GetFloorGenerationAttributes().maxHeight, 0, 1, FBM(x * b.GetFloorGenerationAttributes().smoothness, z * b.GetFloorGenerationAttributes().smoothness, b.GetFloorGenerationAttributes().octaves, b.GetFloorGenerationAttributes().persistence));
    }

    public static float NoiseFBM3D(float x, float y, float z, GenerationAttributes attributes)
    {
        return FBM3D(x * attributes.smoothness, y * attributes.smoothness, z * attributes.smoothness, attributes.octaves, attributes.persistence);
    }

    public static float Humidity(int x, int z)
    {
        return FBM(x * biomeGenerationAttributes.smoothness, z * biomeGenerationAttributes.smoothness, biomeGenerationAttributes.octaves, biomeGenerationAttributes.persistence);
    }

    public static float Temperature(int x, int z)
    {
        return FBM(x * biomeGenerationAttributes.smoothness + 10000, z * biomeGenerationAttributes.smoothness + 10000, biomeGenerationAttributes.octaves, biomeGenerationAttributes.persistence);
    }

    public static bool IsCave(int x, int y, int z)
    {
        float caveProbability = Utils.RidgedFractalNoise3D(x, y, z);
        return caveProbability > 0.5;
    }

    public static bool TreeNoise2D(int x, int z)
    {
        return FBM(
            x * treeGenerationAttributes.smoothness,
            z * treeGenerationAttributes.smoothness,
            treeGenerationAttributes.octaves,
            treeGenerationAttributes.persistence
        ) < treeGenerationAttributes.probability && WhichBiome(x, z).HasTrees();
    }

    public static bool IsTreeLog(int x, int y, int z, float height)
    {
        return y > height && y <= height + 5 && TreeNoise2D(x, z);
    }

    public static bool IsTreeTop(int x, int y, int z)
    {
        // Search for tree
        List<(int, int)> trees = new List<(int, int)>();
        for (int xTree = -2; xTree < 3; xTree++)
        {
            for (int zTree = -2; zTree < 3; zTree++)
            {
                //if (xTree == 0 && zTree == 0) continue;
                if (Utils.TreeNoise2D(x + xTree, z + zTree))
                {
                    trees.Add((x + xTree, z + zTree));
                }
            }
        }


        foreach ((int, int) tree in trees)
        {
            // Find three terrain height
            int h = Utils.TerrainHeight(tree.Item1, tree.Item2);

            if (y < h + 3 || y > h + 7 || (x == tree.Item1 && z == tree.Item2 && y < h + 5 && y > h + 7)) continue; // Wrong height

            if (y == h + 3 || y == h + 4) return true;

            // Find position of this leave
            return true; // If leave
        }
        return false;
    }

    public static bool IsCactus(int x, int y, int z, int height)
    {
        return FBM(x * cactusGenerationAttributes.smoothness, z * cactusGenerationAttributes.smoothness, cactusGenerationAttributes.octaves, cactusGenerationAttributes.persistence) < cactusGenerationAttributes.probability && y > height && y <= height + CactusHeight(x, z);
    }

    public static int CactusHeight(int x, int z)
    {
        return (int)Map(cactusHeightGenerationAttributes.minHeight, cactusHeightGenerationAttributes.maxHeight, 0, 9, (int)Mathf.Round((FBM(x * cactusHeightGenerationAttributes.smoothness, z * cactusHeightGenerationAttributes.smoothness, cactusHeightGenerationAttributes.octaves, cactusHeightGenerationAttributes.persistence) % 1f) * 10f));
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

    static float RidgedFractalNoise3D(float x, float y, float z)
    {
        FastNoise noise = new FastNoise();
        noise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noise.SetSeed(WorldGeneration.SEED);
        noise.SetFrequency(0.02f);
        noise.SetFractalType(FastNoise.FractalType.Billow);
        noise.SetFractalOctaves(8);
        noise.SetFractalLacunarity(12.0f);
        noise.SetFractalGain(0.5f);
        return new FastNoise().GetSimplex(x * 6.0f, y * 6.0f, z * 6.0f);
    }

    public static BiomeGenerationInfo GetGenerationInfo(int x, int z)
    {
        float gap = 0.1f;
        float h = Utils.Humidity(x, z);
        float t = Utils.Temperature(x, z);
        Dictionary<Biome, (float, float)> strengths = new Dictionary<Biome, (float, float)>();

        Biome[] biomes = (Biome[])System.Enum.GetValues(typeof(Biome));

        Biome absoluteBiome = WhichBiome(x, z);
        foreach (Biome other in biomes)
        {
            // Get biome whittaker limits:
            WhittakerLimits wo = other.GetWhittakerLimits();

            // Calculate strengths:
            float strengthHumidity = (gap - (Mathf.Abs((wo.MaxHumidity + wo.MinHumidity) / 2.0f - h) - ((wo.MaxHumidity - wo.MinHumidity - gap) / 2.0f))) / gap;
            float strengthTemperature = (gap - (Mathf.Abs((wo.MaxTemperature + wo.MinTemperature) / 2.0f - t) - ((wo.MaxTemperature - wo.MinTemperature - gap) / 2.0f))) / gap;

            // Whittaker borders:
            if (h < gap / 2.0f)
                strengthHumidity = 1.0f;
            if (t < gap / 2.0f)
                strengthTemperature = 1.0f;
            if (h > 1.0f - gap / 2.0f)
                strengthHumidity = 1.0f;
            if (t > 1.0f - gap / 2.0f)
                strengthTemperature = 1.0f;

            // Handle overflow:
            if (strengthHumidity > 1.0f) strengthHumidity = 1.0f;
            if (strengthTemperature > 1.0f) strengthTemperature = 1.0f;
            if (strengthHumidity < 0.0f) strengthHumidity = 0.0f;
            if (strengthTemperature < 0.0f) strengthTemperature = 0.0f;

            strengths.Add(other, (strengthHumidity, strengthTemperature));
        }

        return new BiomeGenerationInfo(absoluteBiome, strengths);
    }
}
