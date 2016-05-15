using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DebugTextureGenerator : MonoBehaviour {
    public event Action finishedPacking = () => { };

    public Material myMat;
    //Simple manipulation of filenames will allow for texture packs

    public static string saveFolderName = "Textures";

    [SerializeField]
    GameObject chunkPrefab;
    
    byte enumCount;

    int individualTextureSize = 128;

    int maximumAtlastSize;

    List<Texture2D> texturesList = new List<Texture2D>();

    static Texture2D atlas;
    public static Rect[] UVs;

    void Start() {
        if (!Directory.Exists(saveFolderName)) {
            Directory.CreateDirectory(saveFolderName);
        }
        //Run UI logic here
        EstablishTextureSize();
        LoadTextures();
    }

    public string textureFullPath(string textureName) {
        return saveFolderName + "/" + textureName + ".png"; //Change to work with pack names
    }

    public void EstablishTextureSize() {
        //Need to manually set the texture size in the pack somewhere in a file name or something. Auto-finding is too tough
        maximumAtlastSize = individualTextureSize * individualTextureSize;
        atlas = new Texture2D(maximumAtlastSize, maximumAtlastSize);
    }

    public Texture2D getTextureFromFileName(string filePath) {
        Texture2D tex = new Texture2D(individualTextureSize, individualTextureSize, TextureFormat.ARGB32, false);
        if (!tex.LoadImage(File.ReadAllBytes(filePath))) { // If you can't load the texture, load an error placeholder
            tex.LoadImage(File.ReadAllBytes(textureFullPath("error")));
        }
        return tex;
    }

    public void LoadTextures() {
        //get array of all enum names
        var enumNames = Enum.GetNames(typeof(BlockTextureNames));
        byte enumCount = (byte)Enum.GetNames(typeof(BlockTextureNames)).Length;
        var errorTexture = getTextureFromFileName(textureFullPath("error"));
        for (int i = 0; i < 256; i++) {
            var fileName = (BlockTextureNames)i;
            string fullPathName = textureFullPath(fileName.ToString());
            if(i < enumCount && File.Exists(fullPathName)) {
                texturesList.Add(getTextureFromFileName(fullPathName));
            }
            else {
                texturesList.Add(errorTexture);   
            }
        }
        PackTextures();
    }

    public void PackTextures() {
        UVs = atlas.PackTextures(texturesList.ToArray(), 0, maximumAtlastSize);
        atlas.filterMode = FilterMode.Point;
        atlas.wrapMode = TextureWrapMode.Clamp;
        
        AssignToChunks();
    }

    public void AssignToChunks() {
        if (chunkPrefab != null) {
            Material newMat = myMat;
            newMat.SetTexture(Shader.PropertyToID("_MainTex"), atlas);
            chunkPrefab.GetComponent<Chunk>().rend.material = newMat;
        }
        Settings.Instance.TexturesLoaded = true;
        Application.LoadLevel(1);
    }

}

public enum BlockTextureNames : byte {
    dirt_side,
    dirt_bottom,
    grass,
    stone
}
