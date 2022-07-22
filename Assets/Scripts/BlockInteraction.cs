using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public Camera camera;

    enum InteractionType {DESTROY, BUILD};
    InteractionType interactionType;

    void Update()
    {
        bool interaction = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        if (interaction) {
            interactionType = Input.GetMouseButtonDown(0) ? InteractionType.DESTROY : InteractionType.BUILD;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, 10))
            {
                string chunkName = hit.collider.gameObject.name;
                float chunkX = hit.collider.gameObject.transform.position.x;
                float chunkY = hit.collider.gameObject.transform.position.y;
                float chunkZ = hit.collider.gameObject.transform.position.z;

                Vector3 hitBlock;
                if (interactionType == InteractionType.DESTROY)
                {
                    hitBlock = hit.point - hit.normal / 2f;
                }
                else
                {
                    hitBlock = hit.point + hit.normal / 2f;
                }

                int blockX = (int)(Mathf.Round(hitBlock.x) - chunkX);
                int blockY = (int)(Mathf.Round(hitBlock.y) - chunkY);
                int blockZ = (int)(Mathf.Round(hitBlock.z) - chunkZ);

                if (World.chunks.TryGetValue(chunkName, out Chunk c))
                {
                    if (interactionType == InteractionType.DESTROY)
                    {
                        c.chunkData[blockX, blockY, blockZ].setType(BlockType.AIR);
                    }
                    else
                    {
                        c.chunkData[blockX, blockY, blockZ].setType(BlockType.STONE);
                    }
                }
                List<string> updates = new List<string> { chunkName};
                if (blockX == 0)
                {
                    updates.Add(World.ChunkName(new Vector3(chunkX - World.chunkSize, chunkY, chunkZ)));
                }
                if (blockX == World.chunkSize - 1)
                {
                    updates.Add(World.ChunkName(new Vector3(chunkX + World.chunkSize, chunkY, chunkZ)));
                }
                if (blockY == 0)
                {
                    updates.Add(World.ChunkName(new Vector3(chunkX, chunkY - World.chunkSize, chunkZ)));
                }
                if (blockY == World.chunkSize - 1)
                {
                    updates.Add(World.ChunkName(new Vector3(chunkX, chunkY + World.chunkSize, chunkZ)));
                }
                if (blockZ == 0)
                {
                    updates.Add(World.ChunkName(new Vector3(chunkX, chunkY, chunkZ - World.chunkSize)));
                }
                if (blockZ == World.chunkSize - 1)
                {
                    updates.Add(World.ChunkName(new Vector3(chunkX, chunkY, chunkZ + World.chunkSize)));
                }

                foreach (string name in updates)
                {
                    if (World.chunks.TryGetValue(name, out c))
                    {
                        DestroyImmediate(c.gameObject.GetComponent<MeshFilter>());
                        DestroyImmediate(c.gameObject.GetComponent<MeshRenderer>());
                        DestroyImmediate(c.gameObject.GetComponent<MeshCollider>());
                        c.DrawImmediate();
                    }
                }
            }
        }
    }
}
