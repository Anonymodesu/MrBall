using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//called by GameManager; currently unused
public class TextureManager : MonoBehaviour {
	[SerializeField]
	private int stage;

	private Dictionary<Vector3, Mesh> meshes;
	
	//apply textures to all ramps in the stage
	public void applyTextures() {
		GameObject playingField = GameObject.Find("Rollercoaster");

		if(playingField != null) { //null when the scene is the main menu
			meshes = new Dictionary<Vector3, Mesh>();
			applyTexture(playingField.transform);
		} else {
			//Debug.Log("attempting to retile menu");
		}
	}
	
	//applies the textures of all the object's children depending on their transform scaling
	public void applyTexture(Transform obj) {
		foreach(Transform child in obj) {
			MeshFilter meshFilter = child.gameObject.GetComponent<MeshFilter>();
			
			if(meshFilter == null) { //this child is an organisational container for other objects
				applyTexture(child);
				
			} else { //is a physical gameObject; these objects are assumed to have no children

				tileTexture(child.gameObject);

			}
		}
	}

	
	//modifies uvs so that the texture's scaling is properly tiled on its surface
	//see https://answers.unity.com/questions/294165/apply-uv-coordinates-to-unity-cube-by-script.html
	//see https://github.com/Dsphar/Cube_Texture_Auto_Repeat_Unity/blob/master/ReCalcCubeTexture.cs
	//only modify cube meshes' uv's, as sphere and capsule meshes rely on GPU Instancing
	private void tileTexture(GameObject obj) {

		//only rescale the cube
		if(obj.GetComponent<BoxCollider>() != null) {

	        Mesh mesh = GetMesh(obj);
	        mesh.uv = SetupUvMap(mesh.uv, obj);
	        mesh.name = "Cube Instance";

	        if (obj.GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode != TextureWrapMode.Repeat)
	        {
	            obj.GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
	        }

			//apply material for the corresponding stage
			Renderer renderer = obj.GetComponent<Renderer>();
			Materials materials = GameObject.Find("Resources").GetComponent<Materials>();
			renderer.sharedMaterial.SetTexture("_MainTex", materials.rampTextures[stage]);
			renderer.sharedMaterial.SetTexture("_DetailAlbedoMap", materials.rampTextures[stage]);
			renderer.sharedMaterial.SetTexture("_BumpMap", materials.rampNormalMaps[stage]);
		}


	}

	private Mesh GetMesh(GameObject obj)
    {
        Mesh mesh;

        #if UNITY_EDITOR

        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        Mesh meshCopy = UnityEngine.Object.Instantiate(meshFilter.sharedMesh);
        mesh = meshFilter.mesh = meshCopy;

        #else
        
        mesh = obj.GetComponent<MeshFilter>().mesh;

        #endif

        return mesh;
    }

    private Vector2[] SetupUvMap(Vector2[] meshUVs, GameObject obj)
    {
        float width = obj.transform.localScale.x / 2;
        float depth = obj.transform.localScale.z / 2;
        float height = obj.transform.localScale.y / 2;

        //Front
        meshUVs[2] = new Vector2(0, height);
        meshUVs[3] = new Vector2(width, height);
        meshUVs[0] = new Vector2(0, 0);
        meshUVs[1] = new Vector2(width, 0);

        //Back
        meshUVs[7] = new Vector2(0, 0);
        meshUVs[6] = new Vector2(width, 0);
        meshUVs[11] = new Vector2(0, height);
        meshUVs[10] = new Vector2(width, height);

        //Left
        meshUVs[19] = new Vector2(depth, 0);
        meshUVs[17] = new Vector2(0, height);
        meshUVs[16] = new Vector2(0, 0);
        meshUVs[18] = new Vector2(depth, height);

        //Right
        meshUVs[23] = new Vector2(depth, 0);
        meshUVs[21] = new Vector2(0, height);
        meshUVs[20] = new Vector2(0, 0);
        meshUVs[22] = new Vector2(depth, height);

        //Top
        meshUVs[4] = new Vector2(width, 0);
        meshUVs[5] = new Vector2(0, 0);
        meshUVs[8] = new Vector2(width, depth);
        meshUVs[9] = new Vector2(0, depth);

        //Bottom
        meshUVs[13] = new Vector2(width, 0);
        meshUVs[14] = new Vector2(0, 0);
        meshUVs[12] = new Vector2(width, depth);
        meshUVs[15] = new Vector2(0, depth);

        return meshUVs;
    }

}
