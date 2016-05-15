using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadChunks : MonoBehaviour {

    const int CHUNKS_PER_FRAME = 20;
    const byte TEMP_MAX = 255;

    List<WorldPos> sortedChunkPositions;
    List<WorldPos> chunkPositionsBasedOnRenderDistance;
    List<WorldPos> essentialChunkPos = new List<WorldPos> { new WorldPos(0,0,0), new WorldPos(-1,0,0), new WorldPos(0,0,-1),
        new WorldPos(0,0,1), new WorldPos(1,0,0), new WorldPos(1,0,1), new WorldPos(-1,0,-1),
    new WorldPos(1,0,-1), new WorldPos(-1,0,1)};

    public World world;
    Transform mainCameraTransform;

    //Proper way to do this - settings singleton with a public var, getter and setter, and a delegate that this signs onto
    //to register changes
    int renderDistanceInChunks;
    int renderDistanceInBlocks;
    const float WIDTH_OF_PRIORITY_CHUNK_LINE = 2.1f;

    int FOV = 90;

    Queue<WorldPos> updateList = new Queue<WorldPos>();
    Queue<WorldPos> buildList = new Queue<WorldPos>();

    Vector3 lastSortedCameraVector;
    const int FRAMES_BETWEEN_SORT_CHECK = 10;
    const int FRAMES_BETWEEN_DELETE_CHECK = 12;

    int deleteChunkTimer = 0;
    int sortChunkTimer = 0;

    void Awake() {
        //TODO: Load RenderDistanceInChunks from settings, then assign a delegate to watch settings for changes
        renderDistanceInChunks = Settings.Instance.RenderDistanceInChunks;
        renderDistanceInBlocks = Settings.Instance.RenderDistanceInBlocks;
        mainCameraTransform = Camera.main.transform;
        PopulateChunkPositions();
        SortChunkPositions(chunkPositionsBasedOnRenderDistance, mainCameraTransform, renderDistanceInBlocks);
    }

    void Update() {
        sortChunkTimer++;
        FindChunksToLoad(essentialChunkPos); //Always, always check and load immediate vicinity before anything.
        if (updateList.Count > 0) {
            LoadAndRenderChunks();
            return;
        }
        if (DeleteChunks()) 
           return;

        if (sortChunkTimer >= FRAMES_BETWEEN_SORT_CHECK) {
            sortChunkTimer = 0;
            // If we get an angle check that shows we have looked to far in a different direction, resort and go again.
            if (TrigExtensions.AngleCheckBetweenTwoVector3XZ(lastSortedCameraVector, mainCameraTransform.forward.normalized) > 7) { //7 is a good degree angle to resort on
                SortChunkPositions(chunkPositionsBasedOnRenderDistance, mainCameraTransform, renderDistanceInBlocks);
            }
        }

        // DEBUG CODE - draw render line
        //var endPos = Camera.main.transform.position + (Camera.main.transform.forward.normalized * renderDistanceInBlocks);
        //Debug.DrawLine(Camera.main.transform.position, endPos, Color.blue);

        
        FindChunksToLoad(sortedChunkPositions);
        FindChunksToLoad(chunkPositionsBasedOnRenderDistance);
        LoadAndRenderChunks();
    }

    void FindChunksToLoad(List<WorldPos> chunkPositions) {
        //Get the position of this gameobject to generate around
        WorldPos playerPos = new WorldPos(
            Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize,
            Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize,
            Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize
            );

        //If there aren't already chunks to render
        if (updateList.Count == 0) {
            //Cycle through the array of positions
            for (int i = 0; i < chunkPositions.Count; i++) {
                //translate the player position and array position into chunk position
                WorldPos newChunkPos = new WorldPos(
                    chunkPositions[i].x * Chunk.chunkSize + playerPos.x,
                    0,
                    chunkPositions[i].z * Chunk.chunkSize + playerPos.z
                    );

                //Get the chunk in the defined position
                Chunk newChunk = world.GetChunk(
                    newChunkPos.x, newChunkPos.y, newChunkPos.z);

                //If the chunk already exists and it's already
                //rendered, continue
                if (newChunk != null && newChunk.rendered)
                    continue;

                //load a column of chunks in this position
                for (int y = -4; y < 4; y++) {

                    for (int x = newChunkPos.x - Chunk.chunkSize; x <= newChunkPos.x + Chunk.chunkSize; x += Chunk.chunkSize) {
                        for (int z = newChunkPos.z - Chunk.chunkSize; z <= newChunkPos.z + Chunk.chunkSize; z += Chunk.chunkSize) {
                            buildList.Enqueue(new WorldPos(
                                x, y * Chunk.chunkSize, z));
                        }
                    }
                    updateList.Enqueue(new WorldPos(
                                newChunkPos.x, y * Chunk.chunkSize, newChunkPos.z));
                }
                if (updateList.Count > CHUNKS_PER_FRAME) //If we are full, no reason to continue searching the blocks
                return;
            }
        }
    }

    void LoadAndRenderChunks() {
        if (buildList.Count != 0) {
            for (int i = 0; i < buildList.Count && i < CHUNKS_PER_FRAME; i++) {
                BuildChunk(buildList.Peek());
                buildList.Dequeue();
            }
            return;
        }

        if (updateList.Count != 0) {
            for (int i = 0; i < updateList.Count && i < CHUNKS_PER_FRAME; i++) {

                Chunk chunk = world.GetChunk(updateList.Peek().x, updateList.Peek().y, updateList.Peek().z);
                if (chunk != null && chunk.rend.enabled)
                    chunk.update = true;
                updateList.Dequeue();
            }
        }
    }

    void BuildChunk(WorldPos pos) {
        if (world.GetChunk(pos.x, pos.y, pos.z) == null)
            world.CreateChunk(pos.x, pos.y, pos.z);
    }

    bool DeleteChunks() {
       if (deleteChunkTimer >= FRAMES_BETWEEN_DELETE_CHECK) {
            var chunksToDelete = new List<WorldPos>();
            var compareDistance = TrigExtensions.HypotenuseLength(renderDistanceInBlocks, renderDistanceInBlocks);
            foreach (var chunk in world.chunks) {
                float distance = Vector3.Distance(
                    new Vector3(chunk.Value.pos.x, 0, chunk.Value.pos.z),
                    new Vector3(transform.position.x, 0, transform.position.z));
                if (distance > (compareDistance + 25)) {
                    chunksToDelete.Add(chunk.Key);
                }
            }

            foreach (var chunk in chunksToDelete)
                world.DestroyChunk(chunk.x, chunk.y, chunk.z);

            deleteChunkTimer = 0;
            return true;
        }

        deleteChunkTimer++;
        return false;
    }

    void PopulateChunkPositions() {
        // Spawn a grid based on render distance
        List<WorldPos> temp = new List<WorldPos>();
        for (int x = -renderDistanceInChunks; x <= renderDistanceInChunks; x++) {
            for (int z = -renderDistanceInChunks; z <= renderDistanceInChunks; z++) {
                temp.Add(new WorldPos(x, 0, z));
            }
        }

        temp.Sort(); // Use our WorldPos IComparer to compare the absolute value of X and Z. Smaller = closer to the player
        temp = temp.Where(x => !x.IsEssentialChunk()).ToList(); //Remove the essential positions from this list so we aren't scanning them twice later.
        chunkPositionsBasedOnRenderDistance = temp;
        DeleteChunks();
    }

    // TODO: DO NOT UPDATE on frames that you sort
    void SortChunkPositions(List<WorldPos> chunkPositions, Transform mainCameraTranform, int renderDistanceInBlocks) {
        var playerPos = mainCameraTranform.position;
        var endPos = playerPos + (mainCameraTranform.forward.normalized * renderDistanceInBlocks);
        sortChunkTimer = 0;
        var tempPos = chunkPositionsBasedOnRenderDistance. 
            Where(x => TrigExtensions.AngleBetweenObjectAndPoint(mainCameraTranform, (Vector3)x + playerPos) > FOV).
            //OrderBy(x => TrigExtensions.DistanceFromPointToLineSegment3DSquared(playerPos, endPos, (Vector3)x + playerPos));
        //tempPos.Where(x => TrigExtensions.DistanceFromPointToLineSegment3DSquared(playerPos, endPos, (Vector3)x + playerPos) < WIDTH_OF_PRIORITY_CHUNK_LINE).
            OrderBy(x => x.GetXZSum()); // TODO: Get this working
        sortedChunkPositions = tempPos.ToList();
        lastSortedCameraVector = mainCameraTranform.forward.normalized;
        updateList.Clear();
        buildList.Clear();
    }
}