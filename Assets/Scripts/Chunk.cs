using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material material;
    public Block[,,] chunkData;
    public Vector3 chunkSize;
    public GameObject gameObject;

    public Chunk(string name, Vector3 position, Material material)
    {
        gameObject = new GameObject(name);
        gameObject.gameObject.transform.position = position;
        this.material = material;
        Build();
    }

    private void Build()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    chunkData[x, y, z] = new Block(GenerateBlockType(x, y, z), position, this, material);
                }
            }
        }
    }

    public void Draw() {
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].Draw();
                }
            }
        }
        CombineQuads();
    }

    private BlockType GenerateBlockType(int x, int y, int z)
    {
        int blockX = (int)gameObject.transform.position.x + x;
        int blockY = (int)gameObject.transform.position.y + y;
        int blockZ = (int)gameObject.transform.position.z + z;
        int height = Utils.GenerateHeight(blockX, blockZ, World.columnHeight * World.chunkSize, 0.001f, 6, 0.7f);
        int stoneHeight = Utils.GenerateHeight(blockX, blockZ, (World.columnHeight * World.chunkSize) - 4, 0.001f, 5, 0.7f);
        if (blockY < 1) return BlockType.BEDROCK;
        if (blockY < 3)
        {
            if (Random.Range(0f, 10f) > blockY * 2.6) return BlockType.BEDROCK;
            else return BlockType.STONE;
        }
        else if (blockY < stoneHeight) return BlockType.STONE;
        else if (blockY < height) return BlockType.DIRT;
        else if (blockY == height) return BlockType.GRASS;
        else return BlockType.AIR;
    }

    private void CombineQuads()
    {
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        foreach (Transform quad in gameObject.transform)
        {
            Object.Destroy(quad.gameObject);
        }
    }
}
