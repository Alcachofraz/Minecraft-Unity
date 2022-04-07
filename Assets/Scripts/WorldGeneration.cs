using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Biome
{
    PLAINS
}

public class FloorGenerationAttributes : GenerationAttributes
{
    public int minHeight;
    public int maxHeight;

    public FloorGenerationAttributes(int minHeight, int maxHeight, float smoothness, int octaves, float persistence) : base(0f, smoothness, octaves, persistence)
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

public static class BiomeMethods
{

    /// <summary>
    /// Get floor generation attributes.
    /// </summary>
    public static FloorGenerationAttributes GetFloorGenerationAttributes(this Biome b)
    {
        //return BlockInfo(b).Item1;
        return b switch
        {
            Biome.PLAINS => new FloorGenerationAttributes(62, 90, 0.002f, 6, 0.7f),
            _ => new FloorGenerationAttributes(0, 0, 0f, 0, 0f)
        };
    }

    /// <summary>
    /// Get generation attributes. 
    /// </summary>
    public static GenerationAttributes GetGenerationAttributes(this Biome b)
    {
        //return BlockInfo(b).Item1;
        return b switch
        {
            Biome.PLAINS => new GenerationAttributes(0.41f, 0.015f, 6, 0.7f),
            _ => new GenerationAttributes(0f, 0f, 0, 0f)
        };
    }
}

public class WorldGeneration
{
    public static int SEED = 32000;

    public static int DIAMOND_MAX_HEIGHT = 16;
    public static GenerationAttributes DIAMOND_ATTRIBUTES = new GenerationAttributes(0.379f, 0.1f, 6, 0.7f);
    public static int GOLD_MAX_HEIGHT = 32;
    public static GenerationAttributes GOLD_ATTRIBUTES = new GenerationAttributes(0.38f, 0.1f, 6, 0.7f);
    public static int REDSTONE_MAX_HEIGHT = 16;
    public static GenerationAttributes REDSTONE_ATTRIBUTES = new GenerationAttributes(0.38f, 0.1f, 6, 0.7f);
    public static int IRON_MAX_HEIGHT = 256;
    public static GenerationAttributes IRON_ATTRIBUTES = new GenerationAttributes(0.38f, 0.1f, 6, 0.7f);
    public static int COAL_MAX_HEIGHT = 256;
    public static GenerationAttributes COAL_ATTRIBUTES = new GenerationAttributes(0.4f, 0.05f, 6, 0.7f);

    public static BlockType Get(int x, int y, int z)
    {
        var floorAttributes = World.biome.GetFloorGenerationAttributes();
        var caveAttributes = World.biome.GetGenerationAttributes();

        int height = Utils.Noise2D(x, z, floorAttributes);
        floorAttributes.maxHeight -= 4;
        int stoneHeight = Utils.Noise2D(x, z, floorAttributes);
        float caveProbability = Utils.Noise3D(x, y, z, caveAttributes);
        // Level 0 -> Bedrock
        if (y < 1) return BlockType.BEDROCK;
        // Give continuity to Perlin generated 3D cave, if due.
        if (caveProbability < caveAttributes.probability) return BlockType.AIR;
        // Level 1 -> 1/2 chance of being Bedrock
        // Level 2 -> 1/4 chance of being Bedrock
        if (y < 3) return (Random.Range(0f, 1f) < 1 / (2 * (double)y)) ? BlockType.BEDROCK : BlockType.STONE;
        // Stone and ores
        if (y < stoneHeight)
        {
            // Coordinates multiplications serve the purpose of generating different noise values. Otherwise, the same value would always be generated.
            if (y < COAL_MAX_HEIGHT && Utils.Noise3D(x + (SEED * 1), y, z + (SEED * 1), COAL_ATTRIBUTES) < COAL_ATTRIBUTES.probability) return BlockType.COAL_ORE;
            if (y < IRON_MAX_HEIGHT && Utils.Noise3D(x + (SEED * 2), y, z + (SEED * 2), IRON_ATTRIBUTES) < IRON_ATTRIBUTES.probability) return BlockType.IRON_ORE;
            if (y < REDSTONE_MAX_HEIGHT && Utils.Noise3D(x + (SEED + 3), y, z + (SEED * 3), REDSTONE_ATTRIBUTES) < REDSTONE_ATTRIBUTES.probability) return BlockType.REDSTONE_ORE;
            if (y < GOLD_MAX_HEIGHT && Utils.Noise3D(x + (SEED * 4), y, z + (SEED * 4), GOLD_ATTRIBUTES) < GOLD_ATTRIBUTES.probability) return BlockType.GOLD_ORE;
            if (y < DIAMOND_MAX_HEIGHT && Utils.Noise3D(x + (SEED * 5), y, z + (SEED * 5), DIAMOND_ATTRIBUTES) < DIAMOND_ATTRIBUTES.probability) return BlockType.DIAMOND_ORE;
            return BlockType.STONE;
        }

        if (y < height) return BlockType.DIRT;
        if (y == height) return BlockType.GRASS;
        return BlockType.AIR;
    }
}
