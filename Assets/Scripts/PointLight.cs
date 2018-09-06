// Our "Sun" script. This script mostly pulls from tutorial two, which showed us
// how to rotate objects and move them, etc.
using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour
{

    // Colour of our PointLight, and the speed at which it rotates.
    public Color color;
    public int speed = 10;

    void Start()
    {
        // Define the colour of the Sun and its position
        this.color = new Color(.5f, .4f, .2f);
        this.transform.position = new Vector3(500.0f, 0.0f, 320.0f);
    }

    void Update()
    {
        // rotate the sun around the centre of the plane
        transform.RotateAround(new Vector3(32.0f, 0.0f, 32.0f), Vector3.forward, speed * Time.deltaTime);
    }
    public Vector3 GetPosition()
    {
        // This returns the position of the sun so that we can pass this to the
        // Phong shader
        return this.transform.position;
    }
}