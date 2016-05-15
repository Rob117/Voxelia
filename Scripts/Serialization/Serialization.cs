using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

/// <summary>
/// Create a file for every chunk that can be loaded individually
/// </summary>
public static class Serialization {

    public static string saveFolderName = "voxeliaGameSaves";

    // Get, or create and then get, a save location based on our world name
    public static string SaveLocation(string worldName) {
        string saveLocation = saveFolderName + "/" + worldName + "/";

        if (!Directory.Exists(saveLocation)) {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation;
    }

    public static string FileName(WorldPos chunkLocation) {
        string fileName = chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
        return fileName;
    }

    public static void SaveChunk(Chunk chunk) {
        //create a dictionary filled with the world position and block types of every block
        //that has been modified in this particular chunk
        Save save = new Save(chunk);
        if (save.blocks.Count == 0) {
            return;
        }

        string saveFile = CreateFilePath(chunk);

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(saveFile, FileMode.Create, FileAccess.Write, FileShare.None);

        formatter.Serialize(stream, save);
        stream.Close();
    }

    public static bool LoadChunk(Chunk chunk) {
        string saveFile = CreateFilePath(chunk);
        if (!File.Exists(saveFile))
            return false;

        IFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFile, FileMode.Open);

        Save save = (Save)formatter.Deserialize(stream);
        foreach (var block in save.blocks) {
            chunk.blocks[block.Key.x, block.Key.y, block.Key.z] = block.Value;
        }

        stream.Close();
        return true;
    }

    public static string CreateFilePath(Chunk chunk) {
        string filePath = SaveLocation(chunk.world.WorldName) + FileName(chunk.pos);
        return filePath;
    }
}
