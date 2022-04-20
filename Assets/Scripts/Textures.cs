using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    AIR, GRASS, SNOW, DIRT, STONE, SAND, SANDSTONE, BEDROCK, LOG, LEAVES, LEAVES_SOLID, IRON_ORE, COAL_ORE, GOLD_ORE, REDSTONE_ORE, DIAMOND_ORE, CACTUS
}

public static class BlockTypeMethods
{
    /*public static (Texture, bool) Info(BlockType b) 
    {
        return b switch
        {
            BlockType.AIR => (new Texture(10f, 0f, 10f, 0f, 10f, 0f), false),
            BlockType.GRASS => (new Texture(0f, 15f, 3f, 15f, 2f, 15f), true),
            BlockType.DIRT => (new Texture(2f, 15f, 2f, 15f, 2f, 15f), true),
            BlockType.STONE => (new Texture(1f, 15f, 1f, 15f, 1f, 15f), true),
            BlockType.SAND => (new Texture(2f, 14f, 2f, 14f, 2f, 14f), true),
            BlockType.BEDROCK => (new Texture(1f, 14f, 1f, 14f, 1f, 14f), true),
            BlockType.LOG => (new Texture(5f, 14f, 4f, 14f, 5f, 14f), true),
            BlockType.LEAVES => (new Texture(4f, 12f, 4f, 12f, 4f, 12f), false),
            BlockType.LEAVES_SOLID => (new Texture(5f, 12f, 5f, 12f, 5f, 12f), true),
            BlockType.DIAMOND_ORE => (new Texture(1f, 15f, 1f, 15f, 1f, 15f), true),
            _ => (new Texture(10f, 0f, 10f, 0f, 10f, 0f), true)
        };
    }*/

    /// <summary>
    /// Returns an object of type Texture, containing information
    /// about the texture of block of type [b].
    /// </summary>
    public static Texture GetTexture(this BlockType b)
    {
        //return BlockInfo(b).Item1;
        return b switch
        {
            BlockType.AIR => new Texture(10f, 0f, 10f, 0f, 10f, 0f),
            BlockType.GRASS => new Texture(0f, 15f, 3f, 15f, 2f, 15f),
            BlockType.SNOW => new Texture(2f, 11f, 4f, 11f, 2f, 15f),
            BlockType.DIRT => new Texture(2f, 15f, 2f, 15f, 2f, 15f),
            BlockType.STONE => new Texture(1f, 15f, 1f, 15f, 1f, 15f),
            BlockType.SAND => new Texture(2f, 14f, 2f, 14f, 2f, 14f),
            BlockType.SANDSTONE => new Texture(0f, 4f, 0f, 3f, 0f, 2f),
            BlockType.BEDROCK => new Texture(1f, 14f, 1f, 14f, 1f, 14f),
            BlockType.LOG => new Texture(5f, 14f, 4f, 14f, 5f, 14f),
            BlockType.LEAVES => new Texture(4f, 12f, 4f, 12f, 4f, 12f),
            BlockType.LEAVES_SOLID => new Texture(5f, 12f, 5f, 12f, 5f, 12f),
            BlockType.COAL_ORE => new Texture(2f, 13f, 2f, 13f, 2f, 13f),
            BlockType.IRON_ORE => new Texture(1f, 13f, 1f, 13f, 1f, 13f),
            BlockType.GOLD_ORE => new Texture(0f, 13f, 0f, 13f, 0f, 13f),
            BlockType.REDSTONE_ORE => new Texture(3f, 12f, 3f, 12f, 3f, 12f),
            BlockType.DIAMOND_ORE => new Texture(2f, 12f, 2f, 12f, 2f, 12f),
            BlockType.CACTUS => new Texture(5f, 11f, 6f, 11f, 7f, 11f),
            _ => new Texture(10f, 0f, 10f, 0f, 10f, 0f)
        };
    }

    /// <summary>
    /// Wether this block type is solid or not.
    /// </summary>
    public static bool IsSolid(this BlockType b)
    {
        //return Info(b).Item2;
        return b switch
        {
            BlockType.AIR => false,
            BlockType.GRASS => true,
            BlockType.DIRT => true,
            BlockType.STONE => true,
            BlockType.SAND => true,
            BlockType.SANDSTONE => true,
            BlockType.BEDROCK => true,
            BlockType.LOG => true,
            BlockType.LEAVES => false,
            BlockType.LEAVES_SOLID => true,
            BlockType.COAL_ORE => true,
            BlockType.IRON_ORE => true,
            BlockType.GOLD_ORE => true,
            BlockType.REDSTONE_ORE => true,
            BlockType.DIAMOND_ORE => true,
            BlockType.CACTUS => false,
            _ => true
        };
    }
}

public class Texture
{
    public Vector2[] bottom;
    public Vector2[] top;
    public Vector2[] side;

    public Texture(float topX, float topY, float sideX, float sideY, float bottomX, float bottomY)
    {
        Vector2 uv00;
        uv00 = new Vector2(topX, topY) / 16f;
        Vector2[] top = TranslateUV(uv00);
        this.top = top;

        uv00 = new Vector2(sideX, sideY) / 16f;
        Vector2[] side = TranslateUV(uv00);
        this.side = side;

        uv00 = new Vector2(bottomX, bottomY) / 16f;
        Vector2[] bottom = TranslateUV(uv00);
        this.bottom = bottom;
    }

    /// <summary>
    /// Returns array of UV coordinates in format 
    /// [uv11, uv01, uv00, uv10], based on uv00 coordinates.
    /// </summary>
    private Vector2[] TranslateUV(Vector2 uv00)
    {
        return new Vector2[] { uv00 + new Vector2(1f, 1f) / 16, uv00 + new Vector2(0f, 1f) / 16, uv00, uv00 + new Vector2(1f, 0f) / 16 };
    }

    /// <summary>
    /// Get UV coordinates of specified side of this texture's block.
    /// </summary>
    public Vector2[] GetSide(CubeSide side)
    {
        return side switch
        {
            CubeSide.TOP => this.top,
            CubeSide.BOTTOM => this.bottom,
            _ => this.side,
        };
    }
}
