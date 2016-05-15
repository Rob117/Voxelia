using UnityEngine;
using System.Collections;
using SimplexNoise;

// Replace this with biome - specific generators for modularity. Have them inherit from a base terrainGen class
public class TerrainGen {

    public TerrainGen() { }

    #region Terrain Block Settings

    //Stone Base Layer
    float stoneBaseHeight = -24;
    float stoneBaseNoise = 0.05f; //distance between peaks
    float stoneBaseNoiseHeight = 4; // max height of peaks, in blocks

    //Mountain
    float stoneMountainHeight = 48; //Very high - max height
    float stoneMountainFrequency = 0.008f; //Very jagged
    float stoneMinHeight = -12; //Lowest the stone can go

    //Dirt
    float dirtBaseHeight = 1; //Minimum depth on top of the rock
    float dirtNoise = 0.04f;
    float dirtNoiseHeight = 3;


    #endregion

    public Chunk ChunkGen(Chunk chunk) {

            for (int x = chunk.pos.x; x < chunk.pos.x + Chunk.chunkSize; x++) {
            for (int z = chunk.pos.z; z < chunk.pos.z + Chunk.chunkSize; z++) {
                chunk = ChunkColumnGen(chunk, x, z);
            }
        }

        chunk = smoothChunk(chunk);
        return chunk;
    }

    public Chunk ChunkColumnGen(Chunk chunk, int x, int z) {
        int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);
        stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));

        if (stoneHeight < stoneMinHeight)
            stoneHeight = Mathf.FloorToInt(stoneMinHeight);

        stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));

        int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);
        dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));

        for (int y = chunk.pos.y; y < chunk.pos.y + Chunk.chunkSize; y++) {
            if (y <= stoneHeight) {
                chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new Block());
            }
            else if (y <= dirtHeight) {
                chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockGrass());
            }
            else {
                chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockAir());
            }

        }

        return chunk;

    }

    /// <summary>
    /// This generates noise for 3D space. Large scale here causes more jagged features.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="scale"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int GetNoise(int x, int y, int z, float scale, int max) {
        
        return Mathf.FloorToInt((float)(Noise.Generate(x * scale, y * scale , z * scale) + 1f) * (max / 2f));
    }

    Chunk smoothChunk(Chunk chunk) {
        //If a chunk is air with two solid opposite sides, set to one of those sides' types
        //if a chunk is solid with four non-solid sides, set to air
        bool[] sides = new bool[4];
        Block currentBlock = new Block();
        for (int y = 0; y < Chunk.chunkSize; y++) {
            for (int x = 0; x < Chunk.chunkSize; x++) {
                for (int z = 0; z < Chunk.chunkSize; z++) {
                    //currentBlock = chunk.blocks[x, y, z];
                    chunk.blocks[x,y,z].SurroundingSideSolidity(chunk, x, y, z, ref sides);
                    //if all transparent, make this air
                    if (!sides[0] && !sides[1] && !sides[2] && !sides[3]) {
                        chunk.blocks[x,y,z] = new BlockAir(); // Possible problem
                        continue;
                    }
                    //if north and south side are solid, make this block the north block's type
                    if (sides[0] && sides[2]) {
                        chunk.blocks[x,y,z] = chunk.getBlock(x, y, z + 1);
                        continue;
                    }
                    //Same, but with east block
                    if (sides[1] && sides[3]) {
                        chunk.blocks[x,y,z] = chunk.getBlock(x + 1, y, z);
                        continue;
                    }
                }
            }
        }


        return chunk;
    }
}
