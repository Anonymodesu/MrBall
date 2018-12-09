# Mr. Ball

An indie 3D puzzle platformer

## Level Design

All ramps should have the 'Rollercoaster' gameobject as the root parent so they are textured correctly and are considered when calculating bounds.

### Quicksave Notes

Adhere to the following, or quicksaves will not load data correctly.

* Cubies should be parented by the 'Cubies' gameobject
* All animated ramps and containers should be placed under the list 'AnimatedRamps' of the player gameobject
* All checkpoints should have different names

### Animating ramps

Animated ramps are for independently animating single ramps. Animation containers are used to sync the animations of its children ramps.

1. Drag the animated ramp or animation container prefab onto the scene.
2. Add animatable ramps as children if using the animation container.
3. Open the animation window.
4. With the animation target selected, click 'Create New Clip'.
5. Add the desired properties you wish to animate.
6. Click 'Record'.
7. Key in the animation by dragging the vertical white line around and modifying the desired properties at each particular frame.
8. Preview the animation by clicking the play button.
9. In 'Assets', make sure the animation clip has 'Loop Time' and 'Loop Pose' checked.
10. Drag the animation clip to 'Animation Clip' under 'Script_Ramp_Animatable' of the animated ramp.

## Resources

### Graphics Optimisation

* [Material property blocks](https://thomasmountainborn.com/2016/05/25/materialpropertyblocks/)
* [More on material property blocks](https://docs.unity3d.com/Manual/GPUInstancing.html)
* [Rendering profiling](https://docs.unity3d.com/Manual/RenderingStatistics.html)
* [Modifying materials in scripts](https://docs.unity3d.com/Manual/MaterialsAccessingViaScript.html)
* [Graphics optimisation manual](https://unity3d.com/learn/tutorials/temas/performance-optimization/optimizing-graphics-rendering-unity-games)
* [Batching](https://docs.unity3d.com/Manual/DrawCallBatching.html?_ga=2.67136977.1927800258.1540965965-747026384.1533985347)
* [Shaders](https://unity3d.com/learn/tutorials/topics/graphics/gentle-introduction-shaders)

The best course of action I've found is to dynamically batch cubes without enabling GPU instancing for cube materials. I recalculate the mesh.UV's for each cube so the cube textures could be properly tiled to account for arbitrary scaling. However, static batching apparently forbids modification of Meshfilter.mesh and GPU instancing requires shared identical meshes. I've considered changing the tiling factor in a manually constructed 6-quad cube instead, but changing the tiling property (in the Standard Shader interface, not UV's) will prevent dynamic batching, outlined [here](https://answers.unity.com/questions/627195/do-tiled-materials-prevent-dynamic-batching.html).

I don't know how to optimise primitive spheres and capsules, since dynamic batching can only occur for meshes with <300 vertices.
I've played around with Material Property Blocks and adding per-instance data to a custom shader, but I don't know how to set normal maps in the shader. In the end, I just enabled GPU instancing, but don't rescale the sphere or capsule meshes. They always should be uniformly scaled anyway, so I just use a high-quality texture for all spheres and capsules across all stages.
