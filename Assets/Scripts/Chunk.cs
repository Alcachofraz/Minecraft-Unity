using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material material;
    public Block[,,] chunkData;
    public Vector3 chunkSize;

    IEnumerator BuildChunk(int sizeX, int sizeY, int sizeZ)
    {
        chunkData = new Block[sizeX, sizeY, sizeZ];
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    chunkData[x, y, z] = new Block(getBlockType(x, y, z), position, this.gameObject, material); 
                }
            }
        }
        for (int z = 0; z < sizeZ; z++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    chunkData[x, y, z].Draw();
                }
            }
            yield return null;
        }
        CombineQuads();
    }

    private BlockType getBlockType(int x, int y, int z)
    {
        if (Random.Range(0f, 1f) < 0.5f)
            return BlockType.GRASS;
        else
            return BlockType.AIR;
    }

    private void CombineQuads()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);

        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        foreach (Transform quad in this.transform)
        {
            Destroy(quad.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BuildChunk((int)chunkSize.x, (int)chunkSize.y, (int)chunkSize.z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
