using UnityEngine;
using System.Collections;

public class DrawTest : MonoBehaviour {
    static Transform lineStart;
    static Transform lineEnd;
    static Transform point;

    public enum MyRole { Start, End, Point };
    public MyRole myRole;

    void Update() {
        if (myRole == MyRole.Point && Input.GetKeyDown(KeyCode.X)) {
            Debug.Log(string.Format("Starting point [{0}], Ending Point [{1}], Point [{2}]", lineStart.position,
                lineEnd.position, point.position));
            var ray = new Ray(
                lineStart.position, TrigExtensions.DirectionBetweenTwoPoints(lineStart.position, lineEnd.position, false));
            Debug.Log(string.Format("Distance from point to line using ray [{0}]", 
                TrigExtensions.DistanceFromPointToLine3D(ray, point.position)));
            Debug.Log(string.Format("Distance with all variables [{0}]", 
                TrigExtensions.DistanceFromPointToLine3D(lineStart.position, lineEnd.position, point.position, false)));
            Debug.Log(string.Format("Distance from point to linesegment [{0}]",
                TrigExtensions.DistanceFromPointToLineSegment3DSquared(lineStart.position, lineEnd.position, point.position)));
            return;
        }
        
    }

    void Start() {
        switch (myRole) {
            case MyRole.Start:
                lineStart = transform;
                break;
            case MyRole.End:
                lineEnd = transform;
                break;
            case MyRole.Point:
                point = transform;
                break;
            default:
                break;
        }
    }

}
