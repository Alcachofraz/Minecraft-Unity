using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public static int columnHeight = 4;
    public static int chunkSize = 16;
    public static int worldSize = 5;
    public static Dictionary<string, Chunk> chunks;

    public static string ChunkName(Vector3 chunk)
    {
        return (int)chunk.x + " " + (int)chunk.y + " " + (int)chunk.z;
    }

    IEnumerator BuildWorld()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    string chunkName = ChunkName(chunkPosition);
                    Chunk chunk = new Chunk(chunkName, chunkPosition, material);
                    chunk.gameObject.transform.parent = this.transform;
                    chunks.Add(chunkName, chunk);
                }
            }
        }
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            chunk.Value.Draw();
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        StartCoroutine(BuildWorld());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
