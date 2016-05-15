using UnityEngine;
using System;
using System.Collections;

public static class TrigExtensions {

    /// <summary>
    /// Return a vector pointing from the start point to the end point.
    /// </summary>
    /// <param name="startingPoint"></param>
    /// <param name="endingPoint"></param>
    /// <param name="normalize"></param>
    /// <returns></returns>
	public static Vector3 DirectionBetweenTwoPoints(Vector3 startingPoint, Vector3 endingPoint, bool normalize) {
        if (normalize)
            return (endingPoint - startingPoint).normalized;
        return (endingPoint - startingPoint);
    }

    public static float DistanceFromPointToLine3D(Ray ray, Vector3 point, bool returnSquareMagnitude = false) {
        if (returnSquareMagnitude)
            return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
        return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
    }

    public static float DistanceFromPointToLine3D(
        Vector3 lineStart, Vector3 lineEnd, Vector3 point, bool returnSquareMagnitude = false ) {
        var direction = DirectionBetweenTwoPoints(lineStart, lineEnd, true);
        var line = new Ray(lineStart, direction);
        return DistanceFromPointToLine3D(line, point, returnSquareMagnitude);
    }

    /// <summary>
    /// Return the distance to a line segment from a point. Square root this result to get actual magnitude.
    /// </summary>
    /// <param name="LinePointA"></param>
    /// <param name="LinePointB"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static float DistanceFromPointToLineSegment3DSquared(Vector3 LinePointA, Vector3 LinePointB, Vector3 point) {
        Vector3 vecAB = LinePointB - LinePointA;
        Vector3 vecPA = LinePointA - point;

        //Length of (the vector of the original line) and (the vector from our point to LineStart)
        float dotProductA = Vector3.Dot(vecAB, vecPA);

        //Closest is LineStart, because our length puts our point "Behind" the start
        if (dotProductA > 0.0f) 
            return Vector3.Dot(vecPA, vecPA);

        Vector3 vecBP = point - LinePointB;
        //Closest is LineEnd, because our length puts our point "Behind" the end
        if (Vector3.Dot(vecAB, vecBP) > 0.0f)
            return Vector3.Dot(vecBP, vecBP);

        //Closest is on the line
        Vector3 closestPoint = vecPA - vecAB * (dotProductA / Vector3.Dot(vecAB, vecAB));
        return Vector3.Dot(closestPoint, closestPoint);
    }

    public static float AngleBetweenObjectAndPoint(Transform myObject, Vector3 point) {
        return (
            Mathf.Abs(
            Vector3.Angle(
                myObject.forward, myObject.position - point)
                ));
    }

    public static float HypotenuseLength(float sideALength, float sideBLength) {
        return Mathf.Sqrt(SquareFloat(sideALength) + SquareFloat(sideBLength));
    }

    public static float SquareFloat(float number) {
        return (number * number);
    }


    /// <summary>
    /// Checks the angle between two vector3s, ignoring Y angle
    /// </summary>
    /// <returns></returns>
    public static float AngleCheckBetweenTwoVector3XZ(Vector3 from, Vector3 to) {
        var v2A = new Vector2(from.x, from.z);
        var v2B = new Vector2(to.x, to.z);

        return Vector2.Angle(v2A, v2B);
    }
}
