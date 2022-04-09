using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Biome
{
    PLAINS,
    DESERT,
    MOUNTAINS
}

public static class BiomeMethods
{
    public static int DIAMOND_MAX_HEIGHT = 16;
    public static GenerationAttributes DIAMOND_ATTRIBUTES = new GenerationAttributes(0.38f, 0.13f, 6, 0.7f);
    public static int GOLD_MAX_HEIGHT = 32;
    public static GenerationAttributes GOLD_ATTRIBUTES = new GenerationAttributes(0.38f, 0.13f, 6, 0.7f);
    public static int REDSTONE_MAX_HEIGHT = 16;
    public static GenerationAttributes REDSTONE_ATTRIBUTES = new GenerationAttributes(0.38f, 0.13f, 6, 0.7f);
    public static int IRON_MAX_HEIGHT = 256;
    public static GenerationAttributes IRON_ATTRIBUTES = new GenerationAttributes(0.39f, 0.13f, 6, 0.7f);
    public static int COAL_MAX_HEIGHT = 256;
    public static GenerationAttributes COAL_ATTRIBUTES = new GenerationAttributes(0.39f, 0.13f, 6, 0.7f);
    public static int BEDROCK_MAX_HEIGHT = 2;
    public static GenerationAttributes BEDROCK_ATTRIBUTES = new GenerationAttributes(0.4f, 0.4f, 6, 0.7f);

    /// <summary>
    /// Get floor generation attributes.
    /// </summary>
    public static TerrainGenerationAttributes GetFloorGenerationAttributes(this Biome b)
    {
        //return BlockInfo(b).Item1;
        return b switch
        {
            Biome.PLAINS => new TerrainGenerationAttributes(62, 90, 0.003f, 6, 0.7f),
            Biome.DESERT => new TerrainGenerationAttributes(62, 90, 0.003f, 6, 0.7f),
            Biome.MOUNTAINS => new TerrainGenerationAttributes(62, 140, 0.003f, 12, 0.7f),
            _ => new TerrainGenerationAttributes(0, 0, 0f, 0, 0f)
        };
    }

    public static BlockType GenerateBlockType(this Biome b, float x, float y, float z)
    {
        TerrainGenerationAttributes floorAttributes = b.GetFloorGenerationAttributes();

        int height = Utils.PerlinNoise2D(x, z, floorAttributes);
        floorAttributes.maxHeight -= 4;
        int stoneHeight = Utils.PerlinNoise2D(x, z, floorAttributes);
        float caveProbability = Utils.PerlinNoise3D(x, y, z, WorldGeneration.caveGenerationAttributes);

        // Level 0 -> Bedrock
        if (y < 1) return BlockType.BEDROCK;
        // Before cave generation, place Bedrock if due (bedrock has priority over any other block).
        if (y <= BEDROCK_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 1), y, z + (WorldGeneration.SEED * 1), BEDROCK_ATTRIBUTES) < BEDROCK_ATTRIBUTES.probability) return BlockType.BEDROCK;
        // Give continuity to Perlin generated 3D cave, if due.
        if (caveProbability > WorldGeneration.caveGenerationAttributes.probability - 0.005 && caveProbability < WorldGeneration.caveGenerationAttributes.probability) return BlockType.AIR;
        // Biome specific generation
        switch (b)
        {
            case Biome.PLAINS:
                // Tree location
                if (y > height && y < height + 6 && Utils.Probability2D(x, z, new GenerationAttributes(0.10f, 0.9f, 1, 0.7f)) < 0.10f)
                    return BlockType.LOG;
                if (y < stoneHeight)
                {
                    // Coordinates multiplications serve the purpose of generating different noise values. Otherwise, the same value would always be generated.
                    if (y <= COAL_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 2), y, z + (WorldGeneration.SEED * 2), COAL_ATTRIBUTES) < COAL_ATTRIBUTES.probability) return BlockType.COAL_ORE;
                    if (y <= IRON_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 3), y, z + (WorldGeneration.SEED * 3), IRON_ATTRIBUTES) < IRON_ATTRIBUTES.probability) return BlockType.IRON_ORE;
                    if (y <= REDSTONE_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED + 4), y, z + (WorldGeneration.SEED * 4), REDSTONE_ATTRIBUTES) < REDSTONE_ATTRIBUTES.probability) return BlockType.REDSTONE_ORE;
                    if (y <= GOLD_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 5), y, z + (WorldGeneration.SEED * 5), GOLD_ATTRIBUTES) < GOLD_ATTRIBUTES.probability) return BlockType.GOLD_ORE;
                    if (y <= DIAMOND_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 6), y, z + (WorldGeneration.SEED * 6), DIAMOND_ATTRIBUTES) < DIAMOND_ATTRIBUTES.probability) return BlockType.DIAMOND_ORE;
                    return BlockType.STONE;
                }

                if (y < height) return BlockType.DIRT;
                if (y == height) return BlockType.GRASS;
                return BlockType.AIR;
            case Biome.DESERT:
                // Cactus location
                if (y > height && y < height + Random.Range(2, 4) && Utils.Probability2D(x, z, new GenerationAttributes(0.15f, 0.9f, 1, 0.7f)) < 0.15f)
                    return BlockType.CACTUS;
                if (y < stoneHeight)
                {
                    // Coordinates multiplications serve the purpose of generating different noise values. Otherwise, the same value would always be generated.
                    if (y <= COAL_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 2), y, z + (WorldGeneration.SEED * 2), COAL_ATTRIBUTES) < COAL_ATTRIBUTES.probability) return BlockType.COAL_ORE;
                    if (y <= IRON_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 3), y, z + (WorldGeneration.SEED * 3), IRON_ATTRIBUTES) < IRON_ATTRIBUTES.probability) return BlockType.IRON_ORE;
                    if (y <= REDSTONE_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED + 4), y, z + (WorldGeneration.SEED * 4), REDSTONE_ATTRIBUTES) < REDSTONE_ATTRIBUTES.probability) return BlockType.REDSTONE_ORE;
                    if (y <= GOLD_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 5), y, z + (WorldGeneration.SEED * 5), GOLD_ATTRIBUTES) < GOLD_ATTRIBUTES.probability) return BlockType.GOLD_ORE;
                    if (y <= DIAMOND_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 6), y, z + (WorldGeneration.SEED * 6), DIAMOND_ATTRIBUTES) < DIAMOND_ATTRIBUTES.probability) return BlockType.DIAMOND_ORE;
                    return BlockType.STONE;
                }

                if (y < height) return BlockType.SANDSTONE;
                if (y == height) return BlockType.SAND;
                return BlockType.AIR;
            case Biome.MOUNTAINS:
                // Tree location
                if (y > height && y < height + 6 && Utils.Probability2D(x, z, new GenerationAttributes(0.10f, 0.9f, 1, 0.7f)) < 0.10f)
                    return BlockType.LOG;
                if (y < stoneHeight)
                {
                    // Coordinates multiplications serve the purpose of generating different noise values. Otherwise, the same value would always be generated.
                    if (y <= COAL_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 2), y, z + (WorldGeneration.SEED * 2), COAL_ATTRIBUTES) < COAL_ATTRIBUTES.probability) return BlockType.COAL_ORE;
                    if (y <= IRON_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 3), y, z + (WorldGeneration.SEED * 3), IRON_ATTRIBUTES) < IRON_ATTRIBUTES.probability) return BlockType.IRON_ORE;
                    if (y <= REDSTONE_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED + 4), y, z + (WorldGeneration.SEED * 4), REDSTONE_ATTRIBUTES) < REDSTONE_ATTRIBUTES.probability) return BlockType.REDSTONE_ORE;
                    if (y <= GOLD_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 5), y, z + (WorldGeneration.SEED * 5), GOLD_ATTRIBUTES) < GOLD_ATTRIBUTES.probability) return BlockType.GOLD_ORE;
                    if (y <= DIAMOND_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 6), y, z + (WorldGeneration.SEED * 6), DIAMOND_ATTRIBUTES) < DIAMOND_ATTRIBUTES.probability) return BlockType.DIAMOND_ORE;
                    return BlockType.STONE;
                }

                if (y < height) return BlockType.DIRT;
                if (y == height) return BlockType.GRASS;
                return BlockType.AIR;
            default:
                return BlockType.AIR;
        }


    }
}