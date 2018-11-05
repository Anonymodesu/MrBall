# Mr. Ball

An indie 3D puzzle platformer

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
