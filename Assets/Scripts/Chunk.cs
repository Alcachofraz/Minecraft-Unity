using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkStatus { CREATED, BUILDING, BUILT, DRAWING, DRAWN, TO_REMOVE, REMOVING, REMOVED }

public class Chunk
{
    public Material material;
    public Block[,,] chunkData;
    public Vector3 chunkSize;
    public GameObject gameObject;

    public ChunkStatus status;

    public Chunk(string name, Vector3 position, Material material)
    {
        gameObject = new GameObject(name);
        gameObject.gameObject.transform.position = position;
        this.material = material;
        status = ChunkStatus.CREATED;
    }

    public IEnumerator Build()
    {
        status = ChunkStatus.BUILDING;
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    try
                    {
                        (BlockType, Biome) generation = WorldGeneration.Get(
                            (int)gameObject.transform.position.x + x,
                            (int)gameObject.transform.position.y + y,
                            (int)gameObject.transform.position.z + z
                        );
                        chunkData[x, y, z] = new Block(generation.Item1, generation.Item2, position, this, material);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            yield return null;
        }
        status = ChunkStatus.BUILT;
    }

    public IEnumerator Draw() 
    {
        status = ChunkStatus.DRAWING;
        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    try
                    {
                        chunkData[x, y, z].Draw();
                    }
                    catch
                    {
                        status = ChunkStatus.REMOVED;
                        yield break;
                    }
                }
            }
            yield return null;
        }
        MeshCollider collider;
        try
        {
            CombineQuads();
            collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
        }
        catch {
            status = ChunkStatus.REMOVED;
            yield break;
        }

        // Remove friction from mesh collider
        collider.material.staticFriction = 0;
        collider.material.dynamicFriction = 0;
        collider.material.bounciness = 0;
        collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
        collider.material.bounceCombine = PhysicMaterialCombine.Minimum;

        status = ChunkStatus.DRAWN;
    }

    public void Remove()
    {
        Object.Destroy(gameObject);
        status = ChunkStatus.REMOVED;
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
