using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainCollider))]
public class DSTerrainGenerator : MonoBehaviour {
    public int squareSize = 5;
    public float maxHeight = 5.0f;
    public float smoothingFactor = 0.5f;
    private TerrainData terrain;
    float[,] heights;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        terrain = transform.GetComponent<TerrainCollider>().terrainData;
        squareSize = terrain.heightmapWidth;
        DiamondSquare();
    }

    private void DiamondSquare() {
        // Create the height array
        heights = new float[squareSize, squareSize];
        float heightRange = maxHeight;
        int stepSize = squareSize - 1;

        // Initialize the four corners of the map to random height values
        heights[0, 0] = Random.value * heightRange;
        heights[stepSize, 0] = Random.value * heightRange;
        heights[stepSize, stepSize] = Random.value * heightRange;
        heights[0, stepSize] = Random.value * heightRange;

        while(stepSize > 1){
            // Perform diamond step
            for (int i = 0; i < squareSize - 1; i += stepSize) {
                for (int j = 0; j < squareSize - 1; j += stepSize) {
                    DiamondStep(i, j, stepSize, heightRange);
                }
            }

            // Perform square step
            for (int i = 0; i < squareSize - 1; i += stepSize) {
                for (int j = i + (stepSize / 2) % stepSize; j < squareSize - 1;
                     j += stepSize / 2) {
                    SquareStep(i, j, stepSize, heightRange);
                }
            }

            stepSize /= 2;
            heightRange -= (heightRange / 2) * smoothingFactor;
        }

        terrain.SetHeights(0, 0, heights);

        return;
    }

    private void DiamondStep(int i, int j, int ss, float hr) {
        float avg = (heights[i, j] + heights[i + ss, j] + heights[i, j + ss] +
            heights[i + ss, j + ss]) / 4.0f;
        avg += Random.value * hr;
        heights[i + (ss / 2), j + (ss / 2)] = avg;
    }

    private void SquareStep(int i, int j, int ss, float hr) {
        float avg = heights[(i + (ss / 2)) % (squareSize - 1), j] +
            heights[(i - (ss / 2) + (squareSize - 1)) % (squareSize - 1), j] +
            heights[i, (j + (ss / 2)) % (squareSize - 1)] +
            heights[i, (j - (ss / 2) + (squareSize - 1)) % (squareSize - 1)] /
            4.0f;
        avg += Random.value * hr;
        heights[i, j] = avg;
    }
}
