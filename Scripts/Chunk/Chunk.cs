using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {

    public Block[,,] blocks = new Block[chunkSize, chunkSize, chunkSize];

    public World world;
    public WorldPos pos;

    public static int chunkSize = 16;

    public bool update = false;
    public bool rendered;

    public Block getBlock(int x, int y, int z) {
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
    }

    public static bool InRange(int index) {
        return !(index < 0 || index >= chunkSize);
    }

    public void SetBlock(int x, int y, int z, Block block) {
        if (InRange(x) && InRange(y) && InRange(z))
            blocks[x, y, z] = block;
        else
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
    }

    MeshFilter filter;
    MeshCollider coll;
    public MeshRenderer rend;

    void Start() {
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
        rend = GetComponent<MeshRenderer>();
    }

    void UpdateChunk(){
        rendered = true;
        MeshData meshData = new MeshData();

        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {
                    var block = blocks[x, y, z];
                    if (block != null)  //Optimize point
                    meshData = blocks[x, y, z].BlockData(this, x, y, z, meshData);
                }
            }
        }

        RenderMesh(meshData);
    }
    /// <summary>
    /// Send the calculated render info to the mesh and collision components
    /// </summary>
    void RenderMesh(MeshData meshData) {

        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();

        filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colliderVertices.ToArray();
        mesh.triangles = meshData.colliderTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }

    void Update() {
        if (update) {
            update = false;
            UpdateChunk();
        }
    }

    // When we load a chunk, but before we load in the player modifications to that chunk
    // We set all the loaded blocks to unmodified
    public void SetBlocksUnmodified() {
        foreach (Block block in blocks) {
            block.changed = false;
        }
    }

}
