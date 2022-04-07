using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class World : MonoBehaviour
{
    public static int PLAINS_MAX_HEIGHT = 80;

    public GameObject player;
    public Material material;
    public static int chunkSize = 16;
    public static int radius = 3;
    public static ConcurrentDictionary<string, Chunk> chunks;
    public static List<string> toRemove = new List<string>();
    Vector3 lastBuiltPosition;
    bool drawing = false;
    bool building = false;

    public static Biome biome = Biome.PLAINS;

    public static string ChunkName(Vector3 chunk)
    {
        return (int)chunk.x + " " + (int)chunk.y + " " + (int)chunk.z;
    }

    IEnumerator BuildRecursiveWorld(Vector3 chunkPosition, int radius)
    {
        building = true;

        int x = (int)chunkPosition.x;
        int y = (int)chunkPosition.y;
        int z = (int)chunkPosition.z;

        BuildChunkAt(chunkPosition);
        yield return null;

        if (--radius < 0) yield break;

        StartBuilding(new Vector3(x, y, z + chunkSize), radius);
        yield return null;
        StartBuilding(new Vector3(x, y, z - chunkSize), radius);
        yield return null;
        StartBuilding(new Vector3(x + chunkSize, y, z), radius);
        yield return null;
        StartBuilding(new Vector3(x - chunkSize, y, z), radius);
        yield return null;
        StartBuilding(new Vector3(x, y + chunkSize, z), radius);
        yield return null;
        StartBuilding(new Vector3(x, y - chunkSize, z), radius);

        building = false;
    }

    void BuildChunkAt(Vector3 position)
    {
        string chunkName = ChunkName(position);
        if (!chunks.TryGetValue(chunkName, out Chunk chunk))
        {
            chunk = new Chunk(chunkName, position, material);
            chunk.gameObject.transform.parent = this.transform;
            chunks.TryAdd(chunk.gameObject.name, chunk);
        }
    }

    IEnumerator RemoveChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            string chunkName = toRemove[i];
            if (chunks.TryGetValue(chunkName, out Chunk chunk))
            {
                Destroy(chunk.gameObject);
                chunks.TryRemove(chunkName, out chunk);
                yield return null;
            }
        }
    }


    IEnumerator DrawChunks()
    {
        drawing = true;
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            if (chunk.Value.status == ChunkStatus.BUILT)
            {
                chunk.Value.Draw();
                yield return null;
            }
            //if (chunk.Value.gameObject && Vector3.Distance(player.transform.position, chunk.Value.gameObject.transform.position) > chunkSize * radius) toRemove.Add(chunk.Key);
        }
        //StartRemoving();
        drawing = false;
    }

    void StartDrawing()
    {
        StartCoroutine(DrawChunks());
    }

    void StartRemoving()
    {
        StartCoroutine(RemoveChunks());
    }

    void StartBuilding(Vector3 chunkPosition, int radius)
    {
        StartCoroutine(BuildRecursiveWorld(chunkPosition, radius));
    }

    Vector3 WhichChunk(Vector3 position)
    {
        Vector3 chunkPosition = new Vector3();
        chunkPosition.x = Mathf.Floor(position.x / chunkSize) * chunkSize;
        chunkPosition.y = Mathf.Floor(position.y / chunkSize) * chunkSize;
        chunkPosition.z = Mathf.Floor(position.z / chunkSize) * chunkSize;
        return chunkPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        player.SetActive(false);
        chunks = new ConcurrentDictionary<string, Chunk>();
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Vector3 playerPosition = player.transform.position;
        player.transform.position = new Vector3(
            playerPosition.x,
            Utils.Noise2D(playerPosition.x, playerPosition.z, Biome.PLAINS.GetFloorGenerationAttributes()) + 1,
            playerPosition.z
        );
        lastBuiltPosition = playerPosition;
        StartBuilding(WhichChunk(lastBuiltPosition), radius);
        StartDrawing();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 movement = playerPosition - lastBuiltPosition;
        if (movement.magnitude > chunkSize)
        {
            lastBuiltPosition = playerPosition;
            StartBuilding(WhichChunk(lastBuiltPosition), radius);
            StartDrawing();
        }
        if (!drawing) StartDrawing();
        if (!building) player.SetActive(true);
    }
}
