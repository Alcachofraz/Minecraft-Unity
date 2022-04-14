using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTest : MonoBehaviour
{
    public static int PLAINS_MAX_HEIGHT = 80;

    public GameObject player;
    public Material material;
    public static int chunkSize = 16;
    public static int radius = 3;
    public static Dictionary<string, Chunk> chunks;
    public static List<string> toRemove = new List<string>();
    Vector3 lastBuiltPosition;
    bool drawing = false;
    bool building = false;

    public float probability = 0.4f;
    public float gap = 0.005f;
    public float smoothness = 0.01f;
    public int octaves = 2;
    public float persistence = 0.7f;

    static public float sprobability = 0.4f;
    static public float sgap = 0.005f;
    static public float ssmoothness = 0.01f;
    static public int soctaves = 2;
    static public float spersistence = 0.7f;

    public static string ChunkName(Vector3 chunk)
    {
        return (int)chunk.x + " " + (int)chunk.y + " " + (int)chunk.z;
    }

    public IEnumerator BuildChunkCube()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int z = 0; z < 8; z++)
                {
                    Vector3 position = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    string chunkName = ChunkName(position);
                    Chunk chunk = new Chunk(chunkName, position, material);
                    chunk.gameObject.transform.parent = this.transform;
                    chunks.Add(chunk.gameObject.name, chunk);
                }
            }
        }
        foreach (KeyValuePair<string, Chunk> chunk in chunks)
        {
            chunk.Value.Draw();
            yield return null;
        }
    }

    void BuildChunkAt(Vector3 position)
    {
        string chunkName = ChunkName(position);
        if (!chunks.TryGetValue(chunkName, out Chunk chunk))
        {
            chunk = new Chunk(chunkName, position, material);
            chunk.gameObject.transform.parent = this.transform;
            chunks.Add(chunk.gameObject.name, chunk);
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


    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<string, Chunk>();
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Vector3 playerPosition = player.transform.position;
        player.transform.position = new Vector3(
            playerPosition.x,
            100,
            //Utils.TerrainHeightGenerate(playerPosition.x, playerPosition.z, Biome.PLAINS.GetFloorGenerationAttributes()) + 1,
            playerPosition.z
        );
        StartCoroutine(BuildChunkCube());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
