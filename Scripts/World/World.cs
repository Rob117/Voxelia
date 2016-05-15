using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Generator;
using SimplexNoise;

public class World : MonoBehaviour {

    public string WorldName = "world";
    public Camera ChunkRenderCam;

    public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
    [SerializeField]
    GameObject chunkPrefab;

    void Awake() {
        ChunkRenderCam = GameObject.FindObjectOfType<Camera>();
        if (Settings.Instance.internalSeed == 0) { // If seed is not set, time seed, make and save
            var r = new System.Random();
            Settings.Instance.internalSeed = (!System.String.IsNullOrEmpty(Settings.Instance.seed)) ? Settings.Instance.seed.GetHashCode() :
                r.Next(); // If not seed, do something random
        }
        Noise.Seed = (1/Settings.Instance.internalSeed);
    }

    //DEBUG USE Texture checker - forces textures to load if the havent
    void Start() {
        if (!Settings.Instance.TexturesLoaded) {
            Application.LoadLevel(0);
        }
    }

    void UpdateIfEqual(int value1, int value2, WorldPos pos) {
        if (value1 == value2) {
            Chunk chunk = GetChunk(pos.x, pos.y, pos.z);
            if (chunk != null)
                chunk.update = true;
        }
    }

    public void CreateChunk(int x, int y, int z) {
        WorldPos worldPos = new WorldPos(x, y, z);

        GameObject newChunkObject = Instantiate(
            chunkPrefab, new Vector3(x, y, z),
            Quaternion.Euler(Vector3.zero))
            as GameObject;

        //newChunkObject.hideFlags = HideFlags.HideInHierarchy;

        Chunk newChunk = newChunkObject.GetComponent<Chunk>();

        newChunk.pos = worldPos;
        newChunk.world = this;

        chunks.Add(worldPos, newChunk);

        // TODO: This can be biomes. Use simplex noise to determine what is and is not a biome?
        // example: generate noise based on chunk location. If <0.05, ice. if 0.07, water, etc etc
        // poll every 200 blocks or so? store answers in a dictionary, and biome type in the chunk w/ save data
        var terrainGen = new TerrainGen();
        newChunk = terrainGen.ChunkGen(newChunk);

        newChunk.SetBlocksUnmodified();

        Serialization.LoadChunk(newChunk);

        /*
        Alternate method: 

        Another option would be to put the SetBlocksUnmodified after loading. 
        Then if there are any modified blocks they are modified since loading 
        and are not included in the existing save file so to save the changes 
        we would deserialize the save file to a variable and then get the current 
        unmodified blocks and add them to the variable then save that. 
        This would mean that we could save less often because we only save when 
        there are new changes to save but saving would require an extra step. 
        How you do this depends a lot on what type of game you're making so consider 
        this part of your game carefully.

        */
    }

    public void DestroyChunk(int x, int y, int z) {
        Chunk chunk = null;
        if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk)) {
            Serialization.SaveChunk(chunk);
            Object.Destroy(chunk.gameObject);
            chunks.Remove(new WorldPos(x, y, z));
        }
    }

    public Chunk GetChunk(int x, int y, int z) {
        WorldPos pos = new WorldPos();
        float multiple = Chunk.chunkSize;
        pos.x = Mathf.FloorToInt(x / multiple) * Chunk.chunkSize;
        pos.y = Mathf.FloorToInt(y / multiple) * Chunk.chunkSize;
        pos.z = Mathf.FloorToInt(z / multiple) * Chunk.chunkSize;

        Chunk containerChunk = null;

        chunks.TryGetValue(pos, out containerChunk);

        return containerChunk;
    }

    public Block GetBlock(int x, int y, int z) {
        Chunk containerChunk = GetChunk(x, y, z);

        if (containerChunk != null) {
            Block block = containerChunk.getBlock(
                x - containerChunk.pos.x,
                y - containerChunk.pos.y,
                z - containerChunk.pos.z);

            return block;
        }
        else
            return new BlockAir();
    }

    public void SetBlock(int x, int y, int z, Block block) {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null) {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;

            UpdateIfEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z));
            UpdateIfEqual(x - chunk.pos.x, Chunk.chunkSize - 1, new WorldPos(x + 1, y, z));
            UpdateIfEqual(y - chunk.pos.y, 0, new WorldPos(x, y - 1, z));
            UpdateIfEqual(y - chunk.pos.y, Chunk.chunkSize - 1, new WorldPos(x, y + 1, z));
            UpdateIfEqual(z - chunk.pos.z, 0, new WorldPos(x, y, z - 1));
            UpdateIfEqual(z - chunk.pos.z, Chunk.chunkSize - 1, new WorldPos(x, y, z + 1));

        }
    }
}
