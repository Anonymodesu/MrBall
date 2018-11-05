using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//called by GameManager; currently unused
public class TextureManager {
	
	private static TextureManager instance = null;
	
	public static TextureManager getInstance() {
		if(instance == null) {
			instance = new TextureManager();
		}
		
		return instance;
	}
	
	//apply textures to all ramps in the stage
	public void applyTextures(int stage) {
		GameObject playingField = GameObject.Find("Rollercoaster");

		if(playingField != null) { //null when the scene is the main menu
			applyTexture(playingField.transform, stage);
		} else {
			//Debug.Log("attempting to retile menu");
		}
	}
	
	//applies the textures of all the object's children depending on their transform scaling
	public void applyTexture(Transform obj, int stage) {
		foreach(Transform child in obj) {
			MeshFilter meshFilter = child.gameObject.GetComponent<MeshFilter>();
			
			if(meshFilter == null) { //this child is an organisational container for other objects
				applyTexture(child, stage);
				
			} else { //is a physical gameObject; these objects are assumed to have no children
				//renderer.material.mainTextureScale = new Vector2(child.lossyScale.x * 0.66f, child.lossyScale.z * 0.66f);
				//MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
				//renderer.GetPropertyBlock(materialBlock);

				/*
				Materials materials = GameObject.Find("Resources").GetComponent<Materials>();
				switch(stage) { //assign the material corresponding to the stage to all ramps
					case 0: materialBlock.SetTexture("_MainTex", materials.ramp_0); break;
					case 1: materialBlock.SetTexture("_MainTex", materials.ramp_1); break;
					case 2: materialBlock.SetTexture("_MainTex", materials.ramp_2); break;
					case 3: materialBlock.SetTexture("_MainTex", materials.ramp_3); break;
					case 4: materialBlock.SetTexture("_MainTex", materials.ramp_4); break;
					case 5: materialBlock.SetTexture("_MainTex", materials.ramp_5); break;
					case 6: materialBlock.SetTexture("_MainTex", materials.ramp_6); break;
					default: Debug.Log("incorrect stage supplied to texture manager"); break;
				}
				

				switch(child.gameObject.tag) {
					case "Ground": materialBlock.SetColor("_Color", Color.green); break;
					case "Fast": materialBlock.SetColor("_Color", Color.magenta); break;
					case "Bouncy": materialBlock.SetColor("_Color", Color.yellow); break;
					case "Checkpoint": materialBlock.SetColor("_Color", Color.white); break;
					case "Win": materialBlock.SetColor("_Color", Color.red); break;
					case "Perpendicular": materialBlock.SetColor("_Color", new Color(255f/255, 150f/255, 0f/255, 255f/255)); break;
					case "Gravity": materialBlock.SetColor("_Color", Color.cyan); break;
					default: Debug.Log("incorrect tag " + child.gameObject.tag + " supplied to " + child.gameObject.name); break;
				}
				renderer.SetPropertyBlock(materialBlock);
				*/
				tileTexture(child.gameObject, stage);

			}
		}
	}

	
	//modifies uvs so that the texture's scaling is properly tiled on its surface
	//see https://answers.unity.com/questions/294165/apply-uv-coordinates-to-unity-cube-by-script.html
	//only modify cube meshes' uv's, as sphere and capsule meshes rely on GPU Instancing
	private void tileTexture(GameObject obj, int stage) {

		//only the cube has a uv length of 24
		if(obj.GetComponent<MeshFilter>().sharedMesh.uv.Length == 24) {

			Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
			float xScale = obj.transform.lossyScale.x / 2;
			float yScale = obj.transform.lossyScale.y / 2;
			float zScale = obj.transform.lossyScale.z / 2;
					
			//set each uv to the global scale of the object
			//make sure the Texture's wrap mode is set to 'repeat'!
			Vector2[] newUVs = new Vector2[mesh.uv.Length];

			//front face
			newUVs[2] = new Vector2(0,yScale);
			newUVs[3] = new Vector2(xScale, yScale);
			newUVs[0] = new Vector2(0,0);
			newUVs[1] = new Vector2(xScale,0);
					
			//back face
			newUVs[6] = new Vector2(0,yScale);
			newUVs[7] = new Vector2(xScale, yScale);
			newUVs[10] = new Vector2(0,0);
			newUVs[11] = new Vector2(xScale,0);
					
			//left face
			newUVs[17] = new Vector2(0,zScale);
			newUVs[16] = new Vector2(yScale, zScale);
			newUVs[18] = new Vector2(0,0);
			newUVs[19] = new Vector2(yScale,0);
					
			//right face
			newUVs[23] = new Vector2(0,zScale);
			newUVs[22] = new Vector2(yScale, zScale);
			newUVs[20] = new Vector2(0,0);
			newUVs[21] = new Vector2(yScale,0);
					
			//top face
			newUVs[4] = new Vector2(0,zScale);
			newUVs[5] = new Vector2(xScale, zScale);
			newUVs[8] = new Vector2(0,0);
			newUVs[9] = new Vector2(xScale,0);
					
			//bottom face
			newUVs[15] = new Vector2(0,xScale);
			newUVs[14] = new Vector2(zScale, xScale);
			newUVs[12] = new Vector2(0,0);
			newUVs[13] = new Vector2(zScale,0);

			mesh.uv = newUVs;

			//apply material for the corresponding stage
			Renderer renderer = obj.GetComponent<Renderer>();
			Materials materials = GameObject.Find("Resources").GetComponent<Materials>();
			renderer.sharedMaterial.EnableKeyword("_NORMALMAP"); //used to ensure that normal maps are included in the build
			renderer.sharedMaterial.SetTexture("_MainTex", materials.rampTextures[stage]);
			renderer.sharedMaterial.SetTexture("_DetailAlbedoMap", materials.rampTextures[stage]);
			renderer.sharedMaterial.SetTexture("_BumpMap", materials.rampNormalMaps[stage]);
			
		}

		/*
				
		switch(mesh.uv.Length) {
			case 24: // child is a primitive cube
			break;
					
			case 515: //child is a primitive sphere
			for(int i = 0; i < 515; i++) { //scale everything equally
				newUVs[i] = xScale * mesh.uv[i]; //xScale,yScale,zScale are equal in this case
			}
			break;
					
			case 550: //child is a primitive capsule
			for(int i = 0; i < 550; i++) { //scale everything equally
				newUVs[i] = xScale * mesh.uv[i]; //xScale,yScale,zScale are equal in this case
			}
			break;
				
			default: 
			newUVs = mesh.uv;
			Debug.Log(obj.name + " has weird UVs");
			break; //dont do anything
		}
				
		mesh.uv = newUVs;
		*/			
	}

}
