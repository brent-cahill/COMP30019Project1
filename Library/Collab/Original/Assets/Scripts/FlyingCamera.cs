using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
    public float sens = 140;
    public float velocity = 20;
    public TerrainGenerator terrain;
    float distToBound = 3f;
    float distToTerrain = 2f;

    float yaw;
    float pitch=40.0f;

    void Start()
    {
        //Center mouse 
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Mouse();
        WASD();
    }

    void Mouse()
    {
        //Changes pitch and yaw by moving mouse
        yaw += sens * Input.GetAxis("Mouse X") * Time.deltaTime;
        pitch -= sens * Input.GetAxis("Mouse Y") * Time.deltaTime;

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
    void WASD()
    {
        //Changes position with keys 
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (Input.GetKey(KeyCode.W))
        {
            pos += transform.forward * velocity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pos -= transform.forward * velocity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            pos -= transform.right * velocity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos += transform.right * velocity * Time.deltaTime;
        }
        checkBounds(pos);
    }

    void checkBounds(Vector3 pos)
    {
        if (transform.position.x > terrain.heightMapSide- distToBound) {
            pos.x = terrain.heightMapSide- distToBound;
        }
        if (transform.position.x < 0.0f+distToBound)
        {
            pos.x = 0.0f+distToBound;
        }
        if (transform.position.z > terrain.heightMapSide-distToBound)
        {
            pos.z = terrain.heightMapSide-distToBound;
        }
        if (transform.position.z < 0.0f+distToBound)
        {
            pos.z = 0.0f+distToBound;
        }
        if (transform.position.y < distToTerrain)
        {
            pos.y = distToTerrain;
        }

        transform.position = pos;
    }
}
