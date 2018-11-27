using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureManager))]
public class TextureHelper : Editor {

/*
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		TextureManager targetScript = (TextureManager) target;
		if(GUILayout.Button("Tile textures Blyat")) {
			targetScript.applyTextures();
		}
	}
	*/
}
