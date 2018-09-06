using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{

    public Shader shader;
    public PointLight Sun;

    public int size;
    private Color water = new Color(0.5f, 0.8f, 0.95f, 0.25f);
    Vector3[,] waterGrid;

    // Use this for initialization
    void Start()
    {
        // Create the MeshFilter and the MeshRenderer for the terrain
        MeshFilter waterMesh = this.gameObject.AddComponent<MeshFilter>();
        waterMesh.mesh = this.GenerateWater();
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = shader;
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_PointLightColor", Sun.color);
        renderer.material.SetVector("_PointLightPosition", Sun.GetWorldPosition());
    }

    // Simple method to generate the water
    Mesh GenerateWater()
    {
        Mesh m = new Mesh();
        m.name = "Water";

        waterGrid = new Vector3[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                waterGrid[i, j] = new Vector3((float)i, 0f, (float)j);
            }
        }

        // Map the 2D array of vertices back to the 1D array that is Unity
        // convention. Simply follow the same winding order that the vertices
        // would normally go in to create triangles.
        Vector3[] finalVertices = new Vector3[6 * size * size];
        int iter = 0;
        for (int i = 0; i < size - 1; i++)
        {
            for (int j = 0; j < size - 1; j++)
            {
                finalVertices[iter++] = waterGrid[i, j];
                finalVertices[iter++] = waterGrid[i, j + 1];
                finalVertices[iter++] = waterGrid[i + 1, j];
                finalVertices[iter++] = waterGrid[i + 1, j];
                finalVertices[iter++] = waterGrid[i, j + 1];
                finalVertices[iter++] = waterGrid[i + 1, j + 1];
            }
        }
        m.vertices = finalVertices;

        // The colors will all be the water color
        Color[] colors = new Color[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            colors[i] = water;
        }

        m.colors = colors;

        int[] triangles = new int[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            triangles[i] = i;
        }

        m.triangles = triangles;

        // normals are clearly all simply Vector3.up
        Vector3[] normals = new Vector3[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            normals[i] = Vector3.up;
        }
        m.normals = normals;
        return m;
    }
}
