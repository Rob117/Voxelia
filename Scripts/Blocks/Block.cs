using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Block {
    // bool used to determine saving
    public bool changed = true;
    public bool aboveIsSolid = false;
    

    public enum Direction { north, east, south, west, up, down }

    // Where do we start, relatively, on the texture sheet. 
    // 0,0 is the lower left-hand corner of the sheet.

    //Sloppy code - PRODUCTION - remove stone as the base block
    const float tileSize = 0.25f;

    /// <summary>
    /// Calculate UVS -- read the tutorial if this is confusing, it basically
    /// traverses the texture and gets the UVS
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual Vector2[] faceUVs(Direction direction) {
        return GetUVs(BlockTextureNames.stone);
    }

    public Block() {

    }

    /// <summary>
    /// Calculate the Meshdata for this particular block by checking the faces of the surrounding blocks
    /// WARNING: If OVERRIDE, define meshdata.useRenderDataForCol!
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="meshdata"></param>
    /// <returns></returns>
    public virtual MeshData BlockData(Chunk chunk, int x, int y, int z, MeshData meshdata) {
        meshdata.useRenderDataForCol = true;
        //Get the block above you, and check that block's down face
        if (!chunk.getBlock(x, y + 1, z).IsSolid(Direction.down)) {
            aboveIsSolid = false;
            meshdata = FaceDataUp(chunk, x, y, z, meshdata);
        }
        else {
            aboveIsSolid = true;
        }

        if (!chunk.getBlock(x, y - 1, z).IsSolid(Direction.up)) {
            meshdata = FaceDataDown(chunk, x, y, z, meshdata);
        }

        if (!chunk.getBlock(x, y, z + 1).IsSolid(Direction.south)) {
            meshdata = FaceDataNorth(chunk, x, y, z, meshdata);
        }

        if (!chunk.getBlock(x, y, z - 1).IsSolid(Direction.north)) {
            meshdata = FaceDataSouth(chunk, x, y, z, meshdata);
        }

        if (!chunk.getBlock(x + 1, y, z).IsSolid(Direction.west)) {
            meshdata = FaceDataEast(chunk, x, y, z, meshdata);
        }

        if (!chunk.getBlock(x - 1, y, z).IsSolid(Direction.east)) {
            meshdata = FaceDataWest(chunk, x, y, z, meshdata);
        }

        return meshdata;
    }

    /// <summary>
    /// Takes mesh data and adds the up-face verticies and quad data to it
    /// storing it in the class that was passed in.
    /// Then returns the meshdata object.
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="meshData"></param>
    /// <returns></returns>
    protected virtual MeshData FaceDataUp(Chunk chunk, int x, int y, int z, MeshData meshData) {
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(faceUVs(Direction.up));
        return meshData;
    }

    protected virtual MeshData FaceDataDown(Chunk chunk, int x, int y, int z, MeshData meshData) {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(faceUVs(Direction.down));
        return meshData;
    }

    protected virtual MeshData FaceDataNorth(Chunk chunk, int x, int y, int z, MeshData meshData) {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(faceUVs(Direction.north));
        return meshData;
    }

    protected virtual MeshData FaceDataEast(Chunk chunk, int x, int y, int z, MeshData meshData) {
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(faceUVs(Direction.east));
        return meshData;
    }

    protected virtual MeshData FaceDataSouth(Chunk chunk, int x, int y, int z, MeshData meshData) {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(faceUVs(Direction.south));
        return meshData;
    }

    protected virtual MeshData FaceDataWest(Chunk chunk, int x, int y, int z, MeshData meshData) {
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
        meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(faceUVs(Direction.west));
        return meshData;
    }

    public virtual bool IsSolid(Direction direction) {
        switch (direction) {
            case Direction.north:
                return true;
            case Direction.east:
                return true;
            case Direction.south:
                return true;
            case Direction.west:
                return true;
            case Direction.up:
                return true;
            case Direction.down:
                return true;
        }

        return false;
    }

    protected Vector2[] GetUVs(BlockTextureNames enumName) {
        int name = (int)enumName;
        Vector2[] UVs = new Vector2[4];

        UVs[0] = new Vector2(DebugTextureGenerator.UVs[name].x, DebugTextureGenerator.UVs[name].y);
        UVs[3] = new Vector2(DebugTextureGenerator.UVs[name].x + DebugTextureGenerator.UVs[name].width,
            DebugTextureGenerator.UVs[name].y);
        UVs[1] = new Vector2(DebugTextureGenerator.UVs[name].x,
            DebugTextureGenerator.UVs[name].y + DebugTextureGenerator.UVs[name].height);
        UVs[2] = new Vector2(DebugTextureGenerator.UVs[name].x + DebugTextureGenerator.UVs[name].width,
            DebugTextureGenerator.UVs[name].y + DebugTextureGenerator.UVs[name].height);

        return UVs;
    }

    /// <summary>
    /// Get the solidity of the surrounding sides as N,E,S,W
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public void SurroundingSideSolidity(Chunk chunk, int x, int y, int z, ref bool[] array) {
        array[0] = chunk.getBlock(x, y, z + 1).IsSolid(Direction.south);
        array[1] = chunk.getBlock(x + 1, y, z).IsSolid(Direction.west);
        array[2] = chunk.getBlock(x, y, z - 1).IsSolid(Direction.north);
        array[3] = chunk.getBlock(x - 1, y, z).IsSolid(Direction.east);
    }
}
