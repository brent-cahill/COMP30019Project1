using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour
{

    public Color color;
    public int speed = 10;

    void Start()
    {
        this.color = new Color(0.5f, 0.4f, 0.2f);
        this.transform.position = new Vector3(500.0f, 0.0f, 320.0f);
    }

    void Update()
    {
        transform.RotateAround(new Vector3(32.0f, 0.0f, 32.0f), Vector3.forward, speed * Time.deltaTime);
    }
    public Vector3 GetWorldPosition()
    {
        return this.transform.position;
    }
}