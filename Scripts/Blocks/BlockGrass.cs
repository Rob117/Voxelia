using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BlockGrass : Block {
	
    public BlockGrass()
        : base() {

    }

    public override Vector2[] faceUVs(Direction direction) {
        Vector2[] UVs = new Vector2[4];
        switch (direction) {
            case Direction.up:
                //Bot Left
                UVs = GetUVs(BlockTextureNames.grass);
                break;
            case Direction.down:
                UVs = GetUVs(BlockTextureNames.dirt_bottom);
                break;
            default:
                if (!aboveIsSolid)
                    UVs = GetUVs(BlockTextureNames.dirt_side);
                else
                    UVs = GetUVs(BlockTextureNames.dirt_bottom);
                break;
        }
        return UVs;
    }

}
