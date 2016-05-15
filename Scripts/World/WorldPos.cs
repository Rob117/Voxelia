using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct WorldPos : IComparable<WorldPos> {

    public int x, y, z;

    public WorldPos(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public int GetXZSum() {
        return Math.Abs(x) + Math.Abs(z);
    }

    public int CompareTo(WorldPos that) {
        return this.GetXZSum().CompareTo(that.GetXZSum());
    }

    public bool IsEssentialChunk() {
        bool absXisInRange = (Mathf.Abs(x) == 1 || x == 0);
        bool absZisInRange = (Mathf.Abs(z) == 1 || z == 0);
        return (absXisInRange && absZisInRange);
    }
    
    //For getting viewpoint ports to check visibilty
    public static explicit operator Vector3(WorldPos pos) {
        Vector3 vec = new Vector3(pos.x, pos.y, pos.z);
        return vec;
    }

    public static WorldPos operator -(WorldPos A, WorldPos b) {
        return (new WorldPos(A.x - b.x, A.y - b.y, A.z - b.z));
    }

    public override string ToString() {
        string strings = string.Format("{0}, {1}, {2}", x, y, z);
        return strings;
    }

    public override bool Equals(object obj) {
        return (GetHashCode() == obj.GetHashCode());
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 47;

            hash = hash * 227 + x.GetHashCode();
            hash = hash * 277 + y.GetHashCode();
            hash = hash * 277 + z.GetHashCode();

            return hash;
        }
    }

    public static WorldPos GetPosDividedByChunkSize(Vector3 pos) {
        var newX = Mathf.FloorToInt(pos.x / Chunk.chunkSize);
        var newY = Mathf.FloorToInt(pos.y / Chunk.chunkSize);
        var newZ = Mathf.FloorToInt(pos.z / Chunk.chunkSize);

        return new WorldPos(newX, newY, newZ);
    }

    public static WorldPos GetPosDividedByChunkSize(WorldPos pos) {
        var newX = Mathf.FloorToInt(pos.x / Chunk.chunkSize);
        var newY = Mathf.FloorToInt(pos.y / Chunk.chunkSize);
        var newZ = Mathf.FloorToInt(pos.z / Chunk.chunkSize);

        return new WorldPos(newX, newY, newZ);
    }

}
