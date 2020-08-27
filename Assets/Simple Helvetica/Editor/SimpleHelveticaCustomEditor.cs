//Simple Helvetica. Copyright © 2012. Studio Pepwuper, Inc. http://www.pepwuper.com/
//email: info@pepwuper.com
//version 1.0

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(SimpleHelvetica))] 
public class SimpleHelveticaCustomEditor : Editor {
	
	private string PrevFrameText;
	private float PrevFrameCharacterSpacing;
	private float PrevFrameLineSpacing;
	private float PrevFrameSpaceWidth;
	
	
	[MenuItem("GameObject/Create Other/Simple Helvetica", false, 11)]
    static void SimpleHelvetica()
    {
		
		GameObject newSimpleHelvetica = new GameObject ("Simple Helvetica");
		
		//add character models
		GameObject newAlphabets;
		newAlphabets = Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Simple Helvetica/Models/_Alphabets.fbx", typeof(GameObject))) as GameObject;
		newAlphabets.name = "_Alphabets";
		newAlphabets.transform.parent=newSimpleHelvetica.transform;
		
		//add script
		newSimpleHelvetica.AddComponent<SimpleHelvetica>();
		
		//add Mesh Renderer
		newSimpleHelvetica.AddComponent(typeof(MeshRenderer));
		MeshRenderer thisMeshRenderer = newSimpleHelvetica.GetComponent<MeshRenderer>();
		thisMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath("Assets/Simple Helvetica/Materials/Default.mat", typeof(Material)) as Material;
		
		//instantiating prefab
		//GameObject SimpleHelvetica);
		//SimpleHelvetica = AssetDatabase.LoadAssetAtPath("Assets/Simple Helvetica/Simple Helvetica.prefab",typeof(GameObject)) as GameObject; 
		//GameObject newSH = PrefabUtility.InstantiatePrefab(SimpleHelvetica) as GameObject; 
		//PrefabUtility.DisconnectPrefabInstance(newSH);  
				
    }
	
	void Awake(){
		PrevFrameText = (target as SimpleHelvetica).Text;
		PrevFrameCharacterSpacing = (target as SimpleHelvetica).CharacterSpacing;
		PrevFrameLineSpacing = (target as SimpleHelvetica).LineSpacing;
		PrevFrameSpaceWidth = (target as SimpleHelvetica).SpaceWidth;
	}
	
	public override void OnInspectorGUI () {
		
		SimpleHelvetica targetSH = (target as SimpleHelvetica); // find target component
		
		if (!targetSH.enabled){
			
			if(GUILayout.Button("Edit Text", GUILayout.MaxWidth(120))) {
				targetSH.EnableSelf();
	        }
			
		}else{
		
			GUILayout.Label("Text");
			EditorGUILayout.HelpBox("- WARNING: Changing text will reset individual character transform changes\n- Multiple lines supported\n- Undo not supported for this Text area", MessageType.None);
			targetSH.Text = EditorGUILayout.TextArea( (target as SimpleHelvetica).Text);  
			targetSH.CharacterSpacing = EditorGUILayout.FloatField("Character Spacing", (target as SimpleHelvetica).CharacterSpacing);
			targetSH.LineSpacing = EditorGUILayout.FloatField("Line Spacing", (target as SimpleHelvetica).LineSpacing);
			targetSH.SpaceWidth = EditorGUILayout.FloatField("Space Width", (target as SimpleHelvetica).SpaceWidth);
			
			//tell SimpleHelvetica.cs that something has changed (so it runs ManualUpdate() ) when Text | variables are modified
			if (targetSH.Text != PrevFrameText || 
				targetSH.CharacterSpacing != PrevFrameCharacterSpacing ||
				targetSH.LineSpacing != PrevFrameLineSpacing ||
				targetSH.SpaceWidth != PrevFrameSpaceWidth) {
					PrevFrameText = targetSH.Text;
					PrevFrameCharacterSpacing = targetSH.CharacterSpacing;
					PrevFrameLineSpacing = targetSH.LineSpacing;
					PrevFrameSpaceWidth = targetSH.SpaceWidth;
					targetSH.GenerateText();
			}
			
			//DrawDefaultInspector();
			
			/*
			if (!targetSH.UpdateInRealtime){
		        if(GUILayout.Button("Update Text", GUILayout.MaxWidth(120))) {
					targetSH.GenerateText();
		        }
			}
			*/
			EditorGUILayout.Space();
			
			if (!targetSH.BoxColliderAdded){
				if(GUILayout.Button("+ Box Colliders", GUILayout.MaxWidth(120))) {
					targetSH.AddBoxCollider();
		        }
			}
			
			if (targetSH.BoxColliderAdded){
				if(GUILayout.Button("- Box Colliders", GUILayout.MaxWidth(120))) {
					targetSH.RemoveBoxCollider();
		        }
				targetSH.BoxColliderIsTrigger = EditorGUILayout.Toggle( "Is Trigger", (target as SimpleHelvetica).BoxColliderIsTrigger);
				//if(GUILayout.Button("Update Box Collider", GUILayout.MaxWidth(120))) {
					targetSH.SetBoxColliderVariables();
		        //}
			}
			
			EditorGUILayout.Space();
			
			if (!targetSH.RigidbodyAdded){
				if(GUILayout.Button("+ Rigidbody", GUILayout.MaxWidth(120))) {
					targetSH.AddRigidbody();
		        }
			}
			
			if (targetSH.RigidbodyAdded){
								
				if(GUILayout.Button("- Rigidbody", GUILayout.MaxWidth(120))) {
					targetSH.RemoveRigidbody();
		        }
				EditorGUILayout.HelpBox("Press \"Update Rigidbody\" to apply changes\nPress \"Reset Rigidbody\" to revert to default values", MessageType.None);
				
				DrawDefaultInspector();
				
				if(GUILayout.Button("Update Rigidbody", GUILayout.MaxWidth(120))) {
					targetSH.SetRigidbodyVariables();
		        }
				
				if(GUILayout.Button("Reset Rigidbody", GUILayout.MaxWidth(120))) {
					targetSH.ResetRigidbodyVariables();
		        }

			}
			
			
			EditorGUILayout.Space();
			if(GUILayout.Button("Apply\nMesh Renderer\nSettings", GUILayout.MaxWidth(120))) {
				targetSH.ApplyMeshRenderer();
	        }
			EditorGUILayout.HelpBox("Cannot Undo when Applying Mesh Renderer Settings", MessageType.None);
	        
		}
   }
	
	
}
