using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

[Serializable]
public class Settings : Singleton<Settings> {
    public Settings() { }
    //Serialize and de-serialize all this on settings and level changes

    public bool TexturesLoaded =false;

    public int RenderDistanceInChunks = 15;
    public int RenderDistanceInBlocks;
    public string seed = "Kevin"; // Change this to Kevin to watch the madness Unfurl
    public int internalSeed;

    public override void Awake() {
        base.Awake();
        RenderDistanceInBlocks = RenderDistanceInChunks * RenderDistanceInChunks + 1;
    }

}
