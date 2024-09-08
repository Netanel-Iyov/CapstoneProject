using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipNormal : MonoBehaviour
{
    [Range(0f, 0.5f)]
    public float stripWidth = 0.2f;

    [Range(-3f, 3f)]
    public float bottomCutY = -3f;
    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        // Create a new list of triangles
        List<int> newTriangles = new List<int>(triangles);
        // Add reversed triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            newTriangles.Add(triangles[i + 2]);
            newTriangles.Add(triangles[i + 1]);
            newTriangles.Add(triangles[i]);
        }
        mesh.vertices = vertices;
        mesh.triangles = newTriangles.ToArray();
        // Flip normals
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        mesh.normals = normals;

        // Remove strip
        RemoveStrip(mesh);

        // Recalculate bounds and normals
        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
    }

    void RemoveStrip(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            if (Mathf.Abs(v1.y) <= stripWidth || Mathf.Abs(v2.y) <= stripWidth || Mathf.Abs(v3.y) <= stripWidth) continue;

            if (v1.y <= bottomCutY || v2.y <= bottomCutY || v3.y <= bottomCutY)
                continue;
            // Check if any vertex is within the strip
            //if (Mathf.Abs(v1.y) > stripWidth && Mathf.Abs(v2.y) > stripWidth && Mathf.Abs(v3.y) > stripWidth)
            //{
            newTriangles.Add(triangles[i]);
                newTriangles.Add(triangles[i + 1]);
                newTriangles.Add(triangles[i + 2]);
            //}
        }

        mesh.triangles = newTriangles.ToArray();
    }
}