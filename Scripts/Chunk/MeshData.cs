using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData {
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public List<Vector3> colliderVertices = new List<Vector3>();
    public List<int> colliderTriangles = new List<int>();

    public bool useRenderDataForCol;


    /// <summary>
    /// Add triangles for cube shapes
    /// </summary>
    public void AddQuadTriangles() {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        if (useRenderDataForCol) {
            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 3);
            colliderTriangles.Add(colliderVertices.Count - 2);

            colliderTriangles.Add(colliderVertices.Count - 4);
            colliderTriangles.Add(colliderVertices.Count - 2);
            colliderTriangles.Add(colliderVertices.Count - 1);
        }
    }

    public void AddVertex(Vector3 vertex) {
        vertices.Add(vertex);

        if (useRenderDataForCol) {
            colliderVertices.Add(vertex);
        }
    }
    //Study point!
    public void AddTriangle(int tri) {
        triangles.Add(tri);

        if (useRenderDataForCol) {
            colliderTriangles.Add(tri - (vertices.Count - colliderVertices.Count));
        }
    }


    public MeshData() {

    }
}
