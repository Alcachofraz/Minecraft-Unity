using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeSide { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };

public class Block
{
    public BlockType blockType;
    public Biome biome;
    public Chunk owner;
    public Vector3 position;

    public Block(BlockType blockType, Biome biome, Vector3 position, Chunk parent, Material material)
    {
        this.blockType = blockType;
        this.biome = biome;
        this.owner = parent;
        this.position = position;
    }

    void Quad(CubeSide side)
    {
        Mesh mesh = new Mesh();

        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f);

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        int[] triangles;
        Vector2[] uv;

        switch (side)
        {
            case CubeSide.FRONT:
                vertices = new Vector3[] { v4, v5, v1, v0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;
            case CubeSide.BACK:
                vertices = new Vector3[] { v6, v7, v3, v2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                break;
            case CubeSide.TOP:
                vertices = new Vector3[] { v7, v6, v5, v4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { v0, v1, v2, v3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { v5, v6, v2, v1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { v7, v4, v0, v3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;
        }

        triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        uv = blockType.GetTexture().GetSide(side);

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        quad.transform.position = this.position;
        quad.transform.parent = owner.gameObject.transform;

        MeshFilter mf = quad.AddComponent<MeshFilter>();
        mf.mesh = mesh;
    }

    int ConvertToLocalCoordinates(int coord)
    {
        return ((coord %= World.chunkSize) < 0) ? coord + World.chunkSize : coord;
    }

    bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunkData;
        if (x < 0 || x >= World.chunkSize || y < 0 || y >= World.chunkSize || z < 0 || z >= World.chunkSize)
        {
            Vector3 neighbourChunkPosition = owner.gameObject.transform.position 
                + new Vector3(
                    (x - (int)position.x) * World.chunkSize, 
                    (y - (int)position.y) * World.chunkSize, 
                    (z - (int)position.z) *World.chunkSize
                );
            string neighbourChunkName = World.ChunkName(neighbourChunkPosition);
            //if (WorldTest.chunks.TryGetValue(neighbourChunkName, out Chunk neighbourChunk))
            if (World.chunks.TryGetValue(neighbourChunkName, out Chunk neighbourChunk))
                {
                chunkData = neighbourChunk.chunkData;
            }
            else
            {
                return false;
            }
        }
        else
        {
            chunkData = owner.chunkData;
        }
        try
        {
            return chunkData[ConvertToLocalCoordinates(x), ConvertToLocalCoordinates(y), ConvertToLocalCoordinates(z)].blockType.IsSolid();
        }
        catch
        {
            return false;
        }
    }

    public void Draw()
    {
        if (blockType == BlockType.AIR) return;
        if (!HasSolidNeighbour((int)position.x, (int)position.y + 1, (int)position.z))
            Quad(CubeSide.TOP);
        if (!HasSolidNeighbour((int)position.x, (int)position.y - 1, (int)position.z))
            Quad(CubeSide.BOTTOM);
        if (!HasSolidNeighbour((int)position.x + 1, (int)position.y, (int)position.z))
            Quad(CubeSide.RIGHT);
        if (!HasSolidNeighbour((int)position.x - 1, (int)position.y, (int)position.z))
            Quad(CubeSide.LEFT);
        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z + 1))
            Quad(CubeSide.FRONT);
        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z - 1))
            Quad(CubeSide.BACK);
    }
}
