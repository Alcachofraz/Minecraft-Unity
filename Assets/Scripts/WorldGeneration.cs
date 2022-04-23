using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationAttributes : GenerationAttributes
{
    public int minHeight;
    public int maxHeight;

    public TerrainGenerationAttributes(int minHeight, int maxHeight, float smoothness, int octaves, float persistence) : base(0f, smoothness, octaves, persistence)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
    }
}

public class GenerationAttributes
{
    public float probability;
    public float smoothness;
    public int octaves;
    public float persistence;

    public GenerationAttributes(float probability, float smoothness, int octaves, float persistence)
    {
        this.probability = probability;
        this.smoothness = smoothness;
        this.octaves = octaves;
        this.persistence = persistence;
    }
}

public class WorldGeneration
{
    public static int SEED = 32000;

    public static (BlockType, Biome) Get(int x, int y, int z)
    {
        BiomeGenerationInfo info = WhichBiome(x, z);
        return (info.biome.GenerateBlockType(x, y, z, Utils.TerrainHeight(x, z, info)), info.biome);
    }

    public static int GetSpawnHeight(int x, int z)
    {
        return Utils.TerrainHeight(x, z, WhichBiome(x, z)) + 2;
    }

    /*public static int CompareBiomes(Biome b1, Biome b2)
    {
        return (int)(b1.GetWhittakerLimits().min * 100f) - (int)(b2.GetWhittakerLimits().min * 100f);
    }*/

    public static BiomeGenerationInfo WhichBiome(int x, int z)
    {
        float h = Utils.Humidity(x, z);
        float t = Utils.Temperature(x, z);
        Biome biome = Biome.PLAINS;
        Dictionary<Biome, (float, float)> strengths = new Dictionary<Biome, (float, float)>();

        Biome[] biomes = (Biome[])System.Enum.GetValues(typeof(Biome));
        //System.Array.Sort(biomes, CompareBiomes);

        float sum = 0.0f;

        // Find strengths:
        foreach (Biome b in biomes) {
            WhittakerLimits w = b.GetWhittakerLimits();
            if (h > w.MinHumidity && h < w.MaxHumidity && t > w.MinTemperature && t < w.MaxTemperature) {
                biome = b;
                foreach (Biome other in biomes) {
                    if (other == b) continue;
                    WhittakerLimits wo = other.GetWhittakerLimits();
                    float strengthHumidity = 1.0f - (Mathf.Abs((wo.MaxHumidity + wo.MinHumidity) / 2.0f - h) - ((wo.MaxHumidity - wo.MinHumidity) / 2.0f - 0.05f)) * 10.0f;
                    float strengthTemperature = 1.0f - (Mathf.Abs((wo.MaxTemperature + wo.MinTemperature) / 2.0f - t) - ((wo.MaxTemperature - wo.MinTemperature) / 2.0f - 0.05f)) * 10.0f;
                    sum += strengthHumidity;
                    sum += strengthTemperature;
                    strengths[other] = (strengthHumidity, strengthTemperature);
                }
            }
        }

        // Normalize strengths:
        foreach (KeyValuePair<Biome, (float, float)> item in strengths) {
            strengths[item.Key] = (item.Value.Item1 / sum, item.Value.Item2 / sum);
        }

        return new BiomeGenerationInfo(biome, strengths);

        /*for (int i = 0; i < biomes.Length; i++)
        { // For each biome
            if (humidity < biomes[i].GetWhittakerLimits().MaxHumidity && humidity > biomes[i].GetWhittakerLimits().MinHumidity)
            { // If humidity matches
                biome = biomes[i];
                if (biomeRate > biomes[i].GetWhittakerLimits().max - 0.05)
                {
                    if (i == biomes.Length)
                    {
                        strength = 1.0f;
                        topBiome = biome;
                        bottomBiome = biome;
                    }
                    else
                    {
                        strength = (biomeRate - (biomes[i].GetWhittakerLimits().max - 0.05f)) * 10f;
                        topBiome = biomes[i + 1];
                        bottomBiome = biome;
                    }
                }
                else if (biomeRate < biomes[i].GetWhittakerLimits().min + 0.05)
                {
                    if (i == 0)
                    {
                        strength = 1.0f;
                        topBiome = biome;
                        bottomBiome = biome;
                    }
                    else
                    {
                        strength = (biomeRate - (biomes[i].GetWhittakerLimits().min - 0.05f)) * 10f;
                        topBiome = biome;
                        bottomBiome = biomes[i - 1];
                    }
                }
            }
        }

        return new BiomeGenerationInfo(biome, bottomBiome, topBiome, strength);*/
    }
}
