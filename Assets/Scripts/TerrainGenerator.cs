// Here, we use the DiamondSquare algorithm to generate a height map, then use
// that height map for the y component of the vertices that will be used to
// create the actual terrain. The surface normals are calculated via cross-
// product and the vertex normals are calculated from those. Finally, colour is
// added based on the height of the vertex at that location.
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    public Shader shader;
    public PointLight Sun;

    // We use public parameters so that we can set them in our text editor and
    // also adjust them easily in Unity to get the most realistic results
    // possible
    public int heightMapSide = 65;
    public float maxHeight = 25;
    public float minHeight = -25;
    public float heightRange = 20;

    // Colours for the vertices that we will later assign based on height.
    private static Color beach = new Color(0.75f, 0.7f, 0.5f);
    private static Color grass = new Color(0.2f, 0.7f, 0.2f);
    private static Color dirt = new Color(0.8f, 0.5f, 0.25f);
    private static Color snow = new Color(1.0f, 1.0f, 1.0f);

    // 2D grid that we will store the heights in, and 2D Vector3 array that we
    // will store the vertices in before mapping from 2D to 1D. Using 2D arrays
    // here is significantly more intuitive, and mapping them back to 1D is a
    // simple O(n) operation, where n is the total number of vertices.
    float[,] heightMap;
    Vector3[,] vertices;

    // Use this for initialization
    void Start()
    {
        // Create the MeshFilter and the MeshRenderer for the terrain
        MeshFilter terrainMesh = this.gameObject.AddComponent<MeshFilter>();
        terrainMesh.mesh = this.DSTerrainGenerator();
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = shader;
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // This ensures that the terrain is correctly shaded based on the
        // current world position and color of the sun.
        renderer.material.SetColor("_PointLightColor", Sun.color);
        renderer.material.SetVector("_PointLightPosition", Sun.GetPosition());
    }

    // Creates the terrain using the Diamond Square Algorithm
    Mesh DSTerrainGenerator()
    {
        // First create the mesh, and then name it
        Mesh m = new Mesh();
        m.name = "TerrainGenerator";

        // Define the heightMap grid of floats, which will give us our height at
        // a given row and column value, then initialize the corners to random
        // values
        heightMap = new float[heightMapSide, heightMapSide];
        heightMap[0, 0] = Random.Range(minHeight, maxHeight);
        heightMap[0, heightMapSide - 1] = Random.Range(minHeight, maxHeight);
        heightMap[heightMapSide - 1, 0] = Random.Range(minHeight, maxHeight);
        heightMap[heightMapSide - 1, heightMapSide - 1] = Random.Range(minHeight, maxHeight);

        // Perform the DiamondSquare algorithm, defined by a seperate function
        diamondSquare(heightMap, heightMapSide, heightRange);

        // Now we define the vertices as Vector3s, then initialize each vertex
        // to have the location defined by the iterated row, col values, but a
        // height defined by the above heightMap
        vertices = new Vector3[heightMapSide, heightMapSide];
        for (int i = 0; i < heightMapSide; i++)
        {
            for (int j = 0; j < heightMapSide; j++)
            {
                vertices[i, j] = new Vector3(i, heightMap[i, j], j);
            }
        }

        // Calculate surface normal based on the position of the vertices using
        // the cross product. For more information, see here:
        // https://math.stackexchange.com/questions/305642/how-to-find-surface-normal-of-a-triangle
        Vector3[,] surfaceNormals = new Vector3[2 * (heightMapSide - 1), (heightMapSide - 1)];
        for (int i = 0; i < heightMapSide - 1; i++)
        {
            for (int j = 0; j < heightMapSide - 1; j++)
            {
                surfaceNormals[i * 2, j] = Vector3.Normalize(
                    Vector3.Cross((vertices[i, j + 1] - vertices[i, j]),
                                  (vertices[i + 1, j] - vertices[i, j])));
                surfaceNormals[i * 2 + 1, j] = Vector3.Normalize(
                    Vector3.Cross((vertices[i + 1, j + 1] - vertices[i, j + 1]),
                                  (vertices[i + 1, j] - vertices[i, j + 1])));
            }
        }

        // Calculate vertex normal of each vertex using the surface normals that
        // surround the vertex.
        Vector3[,] vertexNormals = new Vector3[heightMapSide, heightMapSide];
        for (int i = 0; i < heightMapSide - 1; i++)
        {
            for (int j = 0; j < heightMapSide - 1; j++)
            {
                vertexNormals[i, j] += surfaceNormals[i * 2, j];
                vertexNormals[i, j + 1] += surfaceNormals[i * 2, j];
                vertexNormals[i + 1, j] += surfaceNormals[i * 2, j];
                vertexNormals[i + 1, j] += surfaceNormals[i * 2 + 1, j];
                vertexNormals[i + 1, j + 1] += surfaceNormals[i * 2 + 1, j];
                vertexNormals[i, j + 1] += surfaceNormals[i * 2 + 1, j];
            }
        }

        // Now we normalize the vertex normals to ensure that they are of length
        // less than 1.
        for (int i = 0; i < heightMapSide; i++)
        {
            for (int j = 0; j < heightMapSide; j++)
            {
                vertexNormals[i, j] = Vector3.Normalize(vertexNormals[i, j]);
            }
        }

        // Map the 2D array of vertices back to the 1D array that is Unity
        // convention. Simply follow the same winding order that the vertices
        // would normally go in to create triangles.
        Vector3[] finalVertices = new Vector3[6 * heightMapSide * heightMapSide];
        int iter = 0;
        for (int i = 0; i < heightMapSide - 1; i++)
        {
            for (int j = 0; j < heightMapSide - 1; j++)
            {
                finalVertices[iter++] = vertices[i, j];
                finalVertices[iter++] = vertices[i, j + 1];
                finalVertices[iter++] = vertices[i + 1, j];
                finalVertices[iter++] = vertices[i + 1, j];
                finalVertices[iter++] = vertices[i, j + 1];
                finalVertices[iter++] = vertices[i + 1, j + 1];
            }
        }

        // Set the mesh vertices to these finalVertices
        m.vertices = finalVertices;

        // Used going forward for iteration
        int numVertices = m.vertices.Length;

        // Define the vertex colours based on the height
        Color[] colors = new Color[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            if (m.vertices[i].y >= 15.0f)
            {
                colors[i] = snow;
            }
            else if (m.vertices[i].y >= 10f)
            {
                colors[i] = dirt;
            }
            else if (m.vertices[i].y >= 2.0f)
            {
                colors[i] = grass;
            }
            else
            {
                colors[i] = beach;
            }
        }

        // Set the mesh colours
        m.colors = colors;

        // Set the triangles of the mesh... relatively straightforward
        int[] triangles = new int[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
            triangles[i] = i;

        m.triangles = triangles;

        // Again, map the 2D array of vertex normals back to the 1D array that
        // is Unity convention. Simply follow the same winding order that the
        // vertices would normally go in to create triangles.
        Vector3[] finalNormals = new Vector3[6 * heightMapSide * heightMapSide];
        iter = 0;
        for (int i = 0; i < heightMapSide - 1; i++)
        {
            for (int j = 0; j < heightMapSide - 1; j++)
            {
                finalNormals[iter++] = vertexNormals[i, j];
                finalNormals[iter++] = vertexNormals[i, j + 1];
                finalNormals[iter++] = vertexNormals[i + 1, j];
                finalNormals[iter++] = vertexNormals[i + 1, j];
                finalNormals[iter++] = vertexNormals[i, j + 1];
                finalNormals[iter++] = vertexNormals[i + 1, j + 1];
            }
        }

        // Set the mesh normals to these final vertexNormals
        m.normals = finalNormals;

        // Used to apply the mesh collider to the terrain
        MeshCollider mCol = GetComponent<MeshCollider>();
        mCol.sharedMesh = m;

        return m;
    }

    // This method implement the core part of Diamond-Square algorithm to
    // generate a heightMap with random values
    void diamondSquare(float[,] heightMap, int size, float heightRange)
    {
        int currSize = size;
        int lastSize = size;
        while (currSize > 2)
        {
            // Iterate through the rows and columns for both the Diamond Step
            // and the Square Step
            for (int i = 0; i < size - 1; i += currSize - 1)
            {
                for (int j = 0; j < size - 1; j += currSize - 1)
                {
                    diamondStep(heightMap, i, j, currSize, heightRange);
                }
            }
            for (int i = 0; i < size - 1; i += currSize - 1)
            {
                for (int j = 0; j < size - 1; j += currSize - 1)
                {
                    squareStep(heightMap, i, j, currSize, heightRange);
                }
            }
            lastSize = currSize;
            currSize = (lastSize + 1) / 2;
            heightRange = heightRange * currSize / lastSize;
        }
    }

    // This is the diamond step which gives the average value at the middle
    // based on the values at the four corners
    void diamondStep(float[,] heightMap, int row, int col, int size, float heightRange)
    {
        int midRow = row + (size - 1) / 2;
        int midCol = col + (size - 1) / 2;
        heightMap[midRow, midCol] = (heightMap[row, col] +
                                     heightMap[row, col + size - 1] +
                                     heightMap[row + size - 1, col] +
                                     heightMap[row + size - 1, col + size - 1]) / 4 +
            Random.Range(-heightRange, heightRange);
        return;
    }

    // This is the square step which gives average values at the middle vertices
    // of each edge based on other values we have already
    void squareStep(float[,] heightMap, int row, int col, int size, float heightRange)
    {
        int midRow = row + (size - 1) / 2;
        int midCol = col + (size - 1) / 2;

        // Check to see if we are at a boundary (i.e., must use only 3 vertices)
        if (col == 0)
        {
            heightMap[midRow, col] = (heightMap[row, col] +
                                      heightMap[row + size - 1, col] +
                                      heightMap[midRow, midCol]) / 3 +
                Random.Range(-heightRange, heightRange);
        }
        else
        {
            heightMap[midRow, col] = (heightMap[row, col] +
                                      heightMap[row + size - 1, col] +
                                      heightMap[midRow, midCol] +
                                      heightMap[midRow, col - (size - 1) / 2]) / 4 +
                Random.Range(-heightRange, heightRange);
        }

        if (row == 0)
        {
            heightMap[row, midCol] = (heightMap[row, col] +
                                      heightMap[row, col + size - 1] +
                                      heightMap[midRow, midCol]) / 3 +
                Random.Range(-heightRange, heightRange);
        }
        else
        {
            heightMap[row, midCol] = (heightMap[row, col] +
                                      heightMap[row, col + size - 1] +
                                      heightMap[midRow, midCol] +
                                      heightMap[row - (size - 1) / 2, midCol]) / 4 +
                Random.Range(-heightRange, heightRange);
        }

        if (col + size - 1 == heightMapSide - 1)
        {
            heightMap[midRow, col + size - 1] = (heightMap[row, col + size - 1] +
                                                 heightMap[row + size - 1, col + size - 1] +
                                                 heightMap[midRow, midCol]) / 3 +
                Random.Range(-heightRange, heightRange);
        }
        else
        {
            heightMap[midRow, col + size - 1] = (heightMap[row, col + size - 1] +
                                                 heightMap[row + size - 1, col + size - 1] +
                                                 heightMap[midRow, midCol] +
                                                 heightMap[midRow, col + size - 1 + (size - 1) / 2]) / 4 +
                Random.Range(-heightRange, heightRange);
        }

        if (row + size - 1 == heightMapSide - 1)
        {
            heightMap[row + size - 1, midCol] = (heightMap[row + size - 1, col] +
                                                 heightMap[row + size - 1, col + size - 1] +
                                                 heightMap[midRow, midCol]) / 3 +
                Random.Range(-heightRange, heightRange);
        }
        else
        {
            heightMap[row + size - 1, midCol] = (heightMap[row + size - 1, col] +
                                                 heightMap[row + size - 1, col + size - 1] +
                                                 heightMap[midRow, midCol] +
                                                 heightMap[row + size - 1 + (size - 1) / 2, midCol]) / 4 +
                Random.Range(-heightRange, heightRange);
        }
    }
}
