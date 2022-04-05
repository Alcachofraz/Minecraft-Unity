using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 2;
    public Dictionary<string, Chunk> chunks;

    public static string ChunkName(Vector3 chunk)
    {
        return (int)chunk.x + " " + (int)chunk.y + " " + (int)chunk.z;
    }

    IEnumerator BuildChunckColumn()
    {
        for (int i = 0; i < columnHeight; i++)
        {
            Vector3 chunkPosition = new Vector3(this.gameObject.transform.position.x, i * chunkSize, this.gameObject.transform.position.z);
            string chunkName = ChunkName(chunkPosition);
            Chunk chunk = new Chunk(chunkName, chunkPosition, material);
            chunk.gameObject.transform.parent = this.transform;
            chunks.Add(chunkName, chunk);
        }

        foreach(KeyValuePair<string, Chunk> chunk in chunks)
        {
            chunk.Value.Draw();
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildChunckColumn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
