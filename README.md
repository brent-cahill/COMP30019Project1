# COMP 30019 Project 1

Implementation of a randomly-generated landscape in Unity, explored using a flight-simulator style camera controls, and illuminated using a custom Cg/HLSL shader implementing the Phong Illumination model.

## Modeling of Fractal Landscape

We created a randomly-generated height map grid using the Diamond-Square algorithm. The heights generated were stored in a 2D array of floats, which were then mapped back to the Unity 1D convention. Finally, the surface normals were then calculated using the cross-product of the vertices, and the vertex normals were calculated from these surface normals.

## Camera Motion

The camera was implemented in a "flight simulator" style, where it can move freely within the bounds of the landscape.  This was done by having the keys "WASD" to move the camera from its current position to a new position with respective key changing the position of the camera in right/left and forward/backward directions, as in gaming standard. In addition, movement of the mouse in x and y directions change the angles of the camera correspondingly. 

To keep the camera inside the bounds of the terrain, the camera script accesses the terrain to retrieve its size and with checkBounds() check that the new camera position is still within the bounds before an update of the position of the camera. Collisions with the terrain were handled by adding a rigid body to the camera as well as a sphere collider and then a mesh collider to the terrain.

## Surface Properties

To give a sense of height that realistically modeled the world, the color of the terrain was generated based on height at that vertex, with beaches near the water, grass in lowlands, dirt in the highlands, and snow on the mountain peaks. All illumination present was created by a "spherical Sun" point light source that orbited around the terrain, simulating night and day. The illumination of the landscape was calculated using the Phong Illumination model via a custom Cg/HLSL shader. Water sections were also present achieved via the creation of a plane passing through the terrain. Another custom Cg/HLSL shader was created for this, also using the Phong illumination model with a higher Ks value, as water is more specularly reflective than land. This shader also created visible waves through the displacement of vertices within the plane.
