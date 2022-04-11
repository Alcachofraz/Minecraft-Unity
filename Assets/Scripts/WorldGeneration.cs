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
    public static GenerationAttributes biomeGenerationAttributes = new GenerationAttributes(0.45f, 0.002f, 6, 0.7f);

    public static (BlockType, Biome) Get(int x, int y, int z)
    {
        float biomeProbability = Utils.Probability2D(x, z, biomeGenerationAttributes);
        Biome biome = (biomeProbability < 0.45f) ? Biome.MOUNTAINS : Biome.PLAINS;
        return (biome.GenerateBlockType(x, y, z), biome);
    }

    public static int GetSpawnHeight(int x, int z)
    {
        float biomeProbability = Utils.Probability2D(x, z, new GenerationAttributes(0.45f, 0.002f, 6, 0.7f));
        Biome b = (biomeProbability < 0.45f) ? Biome.MOUNTAINS : Biome.PLAINS;
        TerrainGenerationAttributes floorAttributes = b.GetFloorGenerationAttributes();
        return Utils.PerlinNoise2D(x, z, floorAttributes);
    }
}
