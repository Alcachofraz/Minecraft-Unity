using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Biome
{
    PLAINS,
    DESERT,
    MOUNTAINS
}

public class BlockLayout
{
    public BlockType surfaceLayer;
    public BlockType outerLayer;
    public BlockType innerLayer;

    public BlockLayout(BlockType surfaceLayer, BlockType outerLayer, BlockType innerLayer)
    {
        this.surfaceLayer = surfaceLayer;
        this.outerLayer = outerLayer;
        this.innerLayer = innerLayer;
    }
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

    public static GenerationAttributes caveGenerationAttributes = new GenerationAttributes(0.0f, 0.005f, 8, 0.5f);

    /// <summary>
    /// Get floor generation attributes.
    /// </summary>
    public static FloorGenerationAttributes GetFloorGenerationAttributes(this Biome b)
    {
        //return BlockInfo(b).Item1;
        return b switch
        {
            Biome.PLAINS => new FloorGenerationAttributes(62, 90, 0.003f, 6, 0.7f),
            Biome.DESERT => new FloorGenerationAttributes(62, 90, 0.003f, 6, 0.7f),
            Biome.MOUNTAINS => new FloorGenerationAttributes(62, 140, 0.003f, 12, 0.7f),
            _ => new FloorGenerationAttributes(0, 0, 0f, 0, 0f)
        };
    }

    /// <summary>
    /// Get generation attributes. 
    /// </summary>
    public static BlockLayout GetBlockLayout(this Biome b)
    {
        //return BlockInfo(b).Item1;
        return b switch
        {
            Biome.PLAINS => new BlockLayout(BlockType.GRASS, BlockType.DIRT, BlockType.STONE),
            Biome.DESERT => new BlockLayout(BlockType.SAND, BlockType.SANDSTONE, BlockType.STONE),
            Biome.MOUNTAINS => new BlockLayout(BlockType.GRASS, BlockType.DIRT, BlockType.STONE),
            _ => new BlockLayout(BlockType.STONE, BlockType.STONE, BlockType.STONE)
        };
    }

    public static BlockType GenerateBlockType(this Biome b, float x, float y, float z)
    {
        FloorGenerationAttributes floorAttributes = b.GetFloorGenerationAttributes();
        BlockLayout blockLayout = b.GetBlockLayout();

        int height = Utils.PerlinNoise2D(x, z, floorAttributes);
        floorAttributes.maxHeight -= 4;
        int stoneHeight = Utils.PerlinNoise2D(x, z, floorAttributes);

        float caveProbability = Utils.PerlinNoise3D(x, y, z, caveGenerationAttributes);

        // Level 0 -> Bedrock
        if (y < 1) return BlockType.BEDROCK;
        // Before cave generation, place Bedrock if due (bedrock has priority over any other block).
        if (y <= BEDROCK_MAX_HEIGHT && Utils.PerlinNoise3D(x + (WorldGeneration.SEED * 1), y, z + (WorldGeneration.SEED * 1), BEDROCK_ATTRIBUTES) < BEDROCK_ATTRIBUTES.probability) return BlockType.BEDROCK;
        // Give continuity to Perlin generated 3D cave, if due.
        if (caveProbability > caveGenerationAttributes.probability - 0.005 && caveProbability < caveGenerationAttributes.probability) return BlockType.AIR;
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
                    return blockLayout.innerLayer;
                }

                if (y < height) return blockLayout.outerLayer;
                if (y == height) return blockLayout.surfaceLayer;
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
                    return blockLayout.innerLayer;
                }

                if (y < height) return blockLayout.outerLayer;
                if (y == height) return blockLayout.surfaceLayer;
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
                    return blockLayout.innerLayer;
                }

                if (y < height) return blockLayout.outerLayer;
                if (y == height) return blockLayout.surfaceLayer;
                return BlockType.AIR;
            default:
                return BlockType.AIR;
        }

        
    }
}

public class WorldGeneration
{
    public static int SEED = 35000;

    public static (BlockType, Biome) Get(int x, int y, int z)
    {
        float biomeProbability = Utils.Probability2D(x, z, new GenerationAttributes(0.45f, 0.002f, 6, 0.7f));
        Biome biome = (biomeProbability < 0.45f) ? Biome.DESERT : Biome.PLAINS;
        return (biome.GenerateBlockType(x, y, z), biome);
    }
}
