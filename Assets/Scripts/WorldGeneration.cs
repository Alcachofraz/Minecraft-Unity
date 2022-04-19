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
    public static int SEED = 35000;

    public static GenerationAttributes caveGenerationAttributes = new GenerationAttributes(0.0f, 0.005f, 8, 0.5f);

    public static (BlockType, Biome) Get(int x, int y, int z)
    {
        BiomeGenerationInfo info = Utils.WhichBiome(x, z);
        return (info.biome.GenerateBlockType(x, y, z, Utils.TerrainHeight(x, z, info)), info.biome);
    }

    public static int GetSpawnHeight(int x, int z)
    {
        return Utils.TerrainHeight(x, z, Utils.WhichBiome(x, z)) + 2;
    }

    public static Biome WhichBiome(int x, int z) {
        float precipitation = Utils.Precipitation(x, z);
        float temperature = Utils.Temperature(x, z);
        
    }
}
