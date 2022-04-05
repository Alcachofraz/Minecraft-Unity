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
                    chunkData[x, y, z] = new Block(getBlockType(x, y, z), position, this, material);
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

    private BlockType getBlockType(int x, int y, int z)
    {
        if (Random.Range(0f, 1f) < 0.5f)
            return BlockType.LOG;
        else
            return BlockType.LEAVES;
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
