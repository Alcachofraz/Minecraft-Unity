using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material material;
    public static int chunkSize = 16;
    public static int radius = 4;
    public static ConcurrentDictionary<string, Chunk> chunks;
    Vector3 lastBuiltPosition;
    bool loaded = false;
    int initialChunkNumber;

    public static string ChunkName(Vector3 chunk)
    {
        return (int)chunk.x + " " + (int)chunk.y + " " + (int)chunk.z;
    }

    void BuildRecursiveWorld(Vector3 chunkPosition, int radius)
    {
        int x = (int)chunkPosition.x;
        int y = (int)chunkPosition.y;
        int z = (int)chunkPosition.z;

        // Build if not built already
        BuildChunkAt(chunkPosition);

        if (--radius < 0) return;

        BuildRecursiveWorld(new Vector3(x, y, z + chunkSize), radius);
        BuildRecursiveWorld(new Vector3(x, y, z - chunkSize), radius);
        BuildRecursiveWorld(new Vector3(x + chunkSize, y, z), radius);
        BuildRecursiveWorld(new Vector3(x - chunkSize, y, z), radius);
        BuildRecursiveWorld(new Vector3(x, y + chunkSize, z), radius);
        BuildRecursiveWorld(new Vector3(x, y - chunkSize, z), radius);
    }

    void BuildChunkAt(Vector3 position)
    {
        string chunkName = ChunkName(position);
        // If chunk doesn't already exist:
        if (!chunks.TryGetValue(chunkName, out Chunk chunk))
        {
            chunk = new Chunk(chunkName, position, material);
            //chunk.gameObject.transform.parent = transform;
            chunks.TryAdd(chunk.gameObject.name, chunk);
            StartCoroutine(chunk.Build());
        }
    }

    void RemoveChunks()
    {
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            // If chunk should be removed:
            if (chunk.Value.status == ChunkStatus.TO_REMOVE)
            {
                chunk.Value.Remove();
                chunks.TryRemove(chunk.Key, out _);
            }
        }
    }


    void DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            // If built but not drawn:
            if (chunk.Value.status == ChunkStatus.BUILT)
            {
                StartCoroutine(chunk.Value.Draw());
            }
            // If far away from player:
            else if (chunk.Value.gameObject && Vector3.Distance(player.transform.position, chunk.Value.gameObject.transform.position) > chunkSize * radius)
            {
                chunk.Value.status = ChunkStatus.TO_REMOVE;
            }
        }
    }

    public static Vector3 WhichChunk(Vector3 position)
    {
        Vector3 chunkPosition = new Vector3();
        chunkPosition.x = Mathf.Floor(position.x / chunkSize) * chunkSize;
        chunkPosition.y = Mathf.Floor(position.y / chunkSize) * chunkSize;
        chunkPosition.z = Mathf.Floor(position.z / chunkSize) * chunkSize;
        return chunkPosition;
    }

    public static int CalculateInitialChunkNumber(int radius)
    {
        int ret = 1 + radius * 4;
        for (int i = 1; i <= radius + 1; i++)
        {
            ret += 2 * (i - 1);
        }
        return ret;
    }

    public static int CountDrawnChunks(int radius)
    {
        int drawn = 0;
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            if (chunk.Value.status == ChunkStatus.DRAWN)
            {
                drawn++;
            }
        }
        return drawn;
    }

    // Start is called before the first frame update
    void Start()
    {
        player.SetActive(false);
        chunks = new ConcurrentDictionary<string, Chunk>();
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Vector3 playerPosition = player.transform.position;
        player.transform.position = new Vector3(
            playerPosition.x + chunkSize / 2,
            WorldGeneration.GetSpawnHeight((int)playerPosition.x, (int)playerPosition.z),
            playerPosition.z + chunkSize / 2
        );
        lastBuiltPosition = player.transform.position;
        BuildRecursiveWorld(WhichChunk(lastBuiltPosition), radius);
        initialChunkNumber = CalculateInitialChunkNumber(radius);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 movement = playerPosition - lastBuiltPosition;
        if (movement.magnitude > chunkSize)
        {
            lastBuiltPosition = playerPosition;
            BuildRecursiveWorld(WhichChunk(lastBuiltPosition), radius);
        }
        DrawChunks();
        RemoveChunks();
        if (CountDrawnChunks(radius) >= initialChunkNumber && !loaded) // Check if initial chunks have been drawn
        {
            player.SetActive(true);
            loaded = true;
        }
    }
}
