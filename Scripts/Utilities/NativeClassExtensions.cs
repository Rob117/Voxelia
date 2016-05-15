using UnityEngine;
using System.Collections;

public static class NativeClassExtensions {
	public static Vector2 LowerRight(this Rect rec) {
        return new Vector2(rec.xMax, rec.yMax);
    }
}
