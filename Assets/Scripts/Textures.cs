using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType { AIR, GRASS, DIRT, STONE }

public static class BlockTypeMethods
{
    /// <summary>
    /// Returns an object of type Texture, containing information
    /// about the texture of block of type [b].
    /// </summary>
    public static Texture GetTexture(this BlockType b)
    {
        return b switch
        {
            /**
            * Block Texture Atlas UV [topX, topY, sideX, sideY, bottomX, bottomY] :
            */
            BlockType.GRASS => new Texture(0f, 15f, 3f, 15f, 2f, 15f),
            BlockType.DIRT => new Texture(2f, 15f, 2f, 15f, 2f, 15f),
            BlockType.STONE => new Texture(1f, 15f, 1f, 15f, 1f, 15f),
            _ => null,
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
        return new Vector2[]{ uv00 + new Vector2(1f, 1f) / 16, uv00 + new Vector2(0f, 1f) / 16, uv00, uv00 + new Vector2(1f, 0f) / 16 };
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
