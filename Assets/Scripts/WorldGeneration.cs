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
        Dictionary<Biome, (float, float)> strengths = new Dictionary<Biome, (float, float)>();
        Dictionary<Biome, (float, float)> normalizedStrengths = new Dictionary<Biome, (float, float)>();

        Biome[] biomes = (Biome[])System.Enum.GetValues(typeof(Biome));
        //System.Array.Sort(biomes, CompareBiomes);

        float sum = 0.0f;

        // Find strengths:
        foreach (Biome other in biomes) {
            WhittakerLimits wo = other.GetWhittakerLimits();
            float strengthHumidity = 1.0f - ((Mathf.Abs(((wo.MaxHumidity + wo.MinHumidity) / 2.0f) - h) - (((wo.MaxHumidity - wo.MinHumidity) / 2.0f) - 0.05f)) * 10.0f);
            float strengthTemperature = 1.0f - ((Mathf.Abs(((wo.MaxTemperature + wo.MinTemperature) / 2.0f) - t) - (((wo.MaxTemperature - wo.MinTemperature) / 2.0f) - 0.05f)) * 10.0f);
            strengthHumidity = strengthHumidity > 1.0f ? 1.0f : (strengthHumidity < 0.0f ? 0.0f : strengthHumidity);
            strengthTemperature = strengthTemperature > 1.0f ? 1.0f : (strengthTemperature < 0.0f ? 0.0f : strengthTemperature);
            if (h < wo.MinHumidity - 0.05 && h > wo.MaxHumidity + 0.05) strengthTemperature = 0.0f;
            sum += strengthHumidity;
            sum += strengthTemperature;
            strengths.Add(other, (strengthHumidity, strengthTemperature));
        }

        (Biome, (float, float)) strongest = (Biome.PLAINS, (0.0f, 0.0f));
        // Normalize strengths and find bsolute biome:
        foreach (KeyValuePair<Biome, (float, float)> item in strengths) {
            if (item.Value.Item1 >= strongest.Item2.Item1 && item.Value.Item2 > strongest.Item2.Item2) strongest = (item.Key, item.Value);
            normalizedStrengths[item.Key] = (item.Value.Item1 / sum, item.Value.Item2 / sum);
        }

        foreach (KeyValuePair<Biome, (float, float)> item in strengths) {
            Debug.Log(item.Key + " - " + item.Value + ", ");
        }

        return new BiomeGenerationInfo(strongest.Item1, normalizedStrengths);

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
