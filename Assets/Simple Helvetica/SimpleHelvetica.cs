//Simple Helvetica. Copyright © 2012. Studio Pepwuper, Inc. http://www.pepwuper.com/
//email: info@pepwuper.com
//version 1.0

using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleHelvetica : MonoBehaviour {
	
	[HideInInspector]
	public string Text = "SIMPLE HELVETICA\n \nby Studio Pepwuper";
	[HideInInspector]
	public float CharacterSpacing = 4f; //spacing between the characters
	[HideInInspector]
	public float LineSpacing = 22f;
	[HideInInspector]
	public float SpaceWidth = 8f; //how wide should the "space" character be?
			
	//box collider variables
	[HideInInspector]
	public bool BoxColliderIsTrigger = false;
	
	//rigidbody variables
	public float Mass=1f;
	public float Drag=0f;
	public float AngularDrag=0.05f;
	public bool UseGravity=true;
	public bool IsKinematic=false;
	public RigidbodyInterpolation Interpolation;
	public CollisionDetectionMode CollisionDetection;
	public bool FreezePositionX=false;
	public bool FreezePositionY=false;
	public bool FreezePositionZ=false;
	public bool FreezeRotationX=false;
	public bool FreezeRotationY=false;
	public bool FreezeRotationZ=false;
	
	private float CharXLocation = 0f;
	private float CharYLocation = 0f;
	
	private Vector3 ObjScale; //the scale of the parent object
	
	//these are used in the Update() to determine if box colliders and rigidbodies should be added during update
	[HideInInspector]
	public bool BoxColliderAdded=false;
	[HideInInspector]
	public bool RigidbodyAdded=false;
	
	void Awake(){
		
#if !UNITY_3_4 && !UNITY_3_5
		//disable _Alphabets and all children under it to remove them from being seen.
		transform.Find("_Alphabets").gameObject.SetActive(false);
		
#else
		//disable _Alphabets and all children under it to remove them from being seen.
		transform.Find("_Alphabets").gameObject.SetActiveRecursively(false);
#endif
		
	}
	
	//Reset is called when the reset button is clicked on the inspector
	void Reset(){
		
		RemoveBoxCollider();
		RemoveRigidbody();
		GenerateText();	
		
	}
	
	
	//Generate New 3D Text
	public void GenerateText() {
		
		//Debug.Log ("GenerateText Called");
		
		ResetText(); //reset before generating new text
		
		//check all letters
		for (int ctr = 0; ctr <= Text.Length - 1; ctr++ ){			
			
			//Debug.Log ("Text Length" + Text.Length);
			//Debug.Log ("ctr"+ctr);
			
			//dealing with linebreaks "\n"
			if ( Text[ctr].ToString().ToCharArray()[0] == "\n"[0] ){
				//Debug.Log ("\\n detected");
				CharXLocation = 0;
				CharYLocation -= LineSpacing;
				continue;
			}
				
			string childObjectName = Text[ctr].ToString();
			
						
			if (childObjectName!=" "){
				
				GameObject LetterToShow;
								
				if (childObjectName =="/"){
					LetterToShow = transform.Find("_Alphabets/"+"slash").gameObject; //special case for "/" since it cannot be used for obj name in fbx					
				} else if (childObjectName=="."){
					LetterToShow = transform.Find("_Alphabets/"+"period").gameObject; //special case for "." - naming issue	
				} else {
					LetterToShow = transform.Find("_Alphabets/"+childObjectName).gameObject;
				}
				
				//Debug.Log(LetterToShow);
				
				AddLetter(LetterToShow);
				
				//find the width of the letter used
				Mesh mesh = LetterToShow.GetComponent<MeshFilter>().sharedMesh;
				Bounds bounds = mesh.bounds;
				CharXLocation += bounds.size.x;
				//Debug.Log (bounds.size.x*ObjScale.x);
			}
			else {
				CharXLocation += SpaceWidth;
			}
		}

#if !UNITY_3_4 && !UNITY_3_5
		//disable child objects inside _Alphabets
		transform.Find("_Alphabets").gameObject.SetActive(false);
#else
		//disable child objects inside _Alphabets
		transform.Find("_Alphabets").gameObject.SetActiveRecursively(false);
#endif
		
	}
	

	void AddLetter(GameObject LetterObject){
		
		GameObject NewLetter = Instantiate(LetterObject, transform.position, transform.rotation) as GameObject;
		NewLetter.transform.parent=transform; //setting parent relationship
		
		//rename instantiated object
		NewLetter.name = LetterObject.name;
		
		//scale accoring to parent obj scale
		float newScaleX = NewLetter.transform.localScale.x*ObjScale.x; 
		float newScaleY = NewLetter.transform.localScale.y*ObjScale.y; 
		float newScaleZ = NewLetter.transform.localScale.z*ObjScale.z; 
		
		Vector3 newScaleAll = new Vector3(newScaleX, newScaleY, newScaleZ);
		NewLetter.transform.localScale = newScaleAll;
		//------------------------------------
		
		//dealing with characters with a line down on the left (kerning, especially for use with multiple lines)
		if (CharXLocation == 0)
			if (NewLetter.name == "B" || 
				NewLetter.name == "D" || 
				NewLetter.name == "E" || 
				NewLetter.name == "F" || 
				NewLetter.name == "H" || 
				NewLetter.name == "I" || 
				NewLetter.name == "K" || 
				NewLetter.name == "L" || 
				NewLetter.name == "M" || 
				NewLetter.name == "N" || 
				NewLetter.name == "P" || 
				NewLetter.name == "R" || 
				NewLetter.name == "U" || 
				NewLetter.name == "b" || 
				NewLetter.name == "h" || 
				NewLetter.name == "i" ||
				NewLetter.name == "k" ||
				NewLetter.name == "l" ||
				NewLetter.name == "m" ||
				NewLetter.name == "n" ||
				NewLetter.name == "p" ||
				NewLetter.name == "r" ||
				NewLetter.name == "u" || 
				NewLetter.name == "|" || 
				NewLetter.name == "[" || 
				NewLetter.name == "!")
				CharXLocation += 2;
		
		//position the new char
		NewLetter.transform.localPosition = new Vector3(CharXLocation,CharYLocation,0);
		
		CharXLocation += CharacterSpacing; //add a small space between words
	}
	
	
	void ResetText(){
		
		//reset scale
		//transform.localScale = new Vector3(1,1,1);
		
		//get object scale
		ObjScale = transform.localScale;
		
		//reset position
		CharXLocation = 0f;
		CharYLocation = 0f;
		
		//remove all previous created letters
		Transform[] previousLetters;
		previousLetters = GetComponentsInChildren<Transform>();
		foreach(Transform childTransform in previousLetters){
			if (childTransform.name != "_Alphabets" && childTransform.name != transform.name && childTransform.parent.name != "_Alphabets"){
				//Debug.Log("previous letter: "+childTransform.name);
				DestroyImmediate(childTransform.gameObject);	
			}
			
		}
		
	}
	
	
	public void AddBoxCollider(){
		//Debug.Log ("AddBoxCollider");
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			child.gameObject.AddComponent<BoxCollider>();
		}
				
		foreach (Transform child in transform){
			if (child.name!="_Alphabets"){
				child.gameObject.AddComponent<BoxCollider>();
			}
		}
		
		BoxColliderAdded = true;
		
		//set previously set values
		SetBoxColliderVariables();
	}
	
	public void RemoveBoxCollider(){
		//Debug.Log ("RemoveBoxCollider");
			
		foreach (Transform child in transform.Find ("_Alphabets")){
			DestroyImmediate(child.gameObject.GetComponent<BoxCollider>());
		}
		
		foreach (Transform child in transform){
			if (child.name!="_Alphabets"){
				DestroyImmediate(child.gameObject.GetComponent<BoxCollider>());
			}
		}
		
		BoxColliderAdded = false;
		
	}
	
	public void AddRigidbody(){
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			child.gameObject.AddComponent<Rigidbody>();
		}
		
		foreach (Transform child in transform){
			if (child.name!="_Alphabets"){
				child.gameObject.AddComponent<Rigidbody>();					
			}
		}
		
		RigidbodyAdded = true;
		
		//apply previously set values
		SetRigidbodyVariables();
	}
	
	public void RemoveRigidbody(){
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			DestroyImmediate(child.gameObject.GetComponent<Rigidbody>());
		}
		
		foreach (Transform child in transform){
			if (child.name!="_Alphabets"){
				DestroyImmediate(child.gameObject.GetComponent<Rigidbody>());					
			}
		}
		
		RigidbodyAdded=false;
		
	}
	
	public void SetBoxColliderVariables(){
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			BoxCollider thisCollider = child.gameObject.GetComponent<BoxCollider>();
			if (thisCollider!=null){
				thisCollider.isTrigger = BoxColliderIsTrigger;
			}
		}
		
		foreach (Transform child in transform){
			BoxCollider thisCollider = child.gameObject.GetComponent<BoxCollider>();
			if (child.name!="_Alphabets" && thisCollider!=null){
				thisCollider.isTrigger = BoxColliderIsTrigger;
			}
		}
		
	}
	
	public void SetRigidbodyVariables(){
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			Rigidbody thisRigidbody = child.gameObject.GetComponent<Rigidbody>();
			if (thisRigidbody!=null){
				thisRigidbody.mass = Mass;
				thisRigidbody.drag = Drag;
				thisRigidbody.angularDrag = AngularDrag;
				thisRigidbody.useGravity = UseGravity;
				thisRigidbody.isKinematic = IsKinematic;
				thisRigidbody.interpolation = Interpolation;
				thisRigidbody.collisionDetectionMode = CollisionDetection;
				
				if (FreezePositionX)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezePositionX;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
				if (FreezePositionY)
					thisRigidbody.constraints|=RigidbodyConstraints.FreezePositionY;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
				if (FreezePositionZ)
					thisRigidbody.constraints|=RigidbodyConstraints.FreezePositionZ;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
				if (FreezeRotationX)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezeRotationX;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
				if (FreezeRotationY)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezeRotationY;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
				if (FreezeRotationZ)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezeRotationZ;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;				
			
			}
		}
		
		foreach (Transform child in transform){
			Rigidbody thisRigidbody = child.gameObject.GetComponent<Rigidbody>();
			if (child.name!="_Alphabets" && thisRigidbody!=null){
				thisRigidbody.mass = Mass;
				thisRigidbody.drag = Drag;
				thisRigidbody.angularDrag = AngularDrag;
				thisRigidbody.useGravity = UseGravity;
				thisRigidbody.isKinematic = IsKinematic;
				thisRigidbody.interpolation = Interpolation;
				thisRigidbody.collisionDetectionMode = CollisionDetection;
				
				if (FreezePositionX)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezePositionX;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
				if (FreezePositionY)
					thisRigidbody.constraints|=RigidbodyConstraints.FreezePositionY;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
				if (FreezePositionZ)
					thisRigidbody.constraints|=RigidbodyConstraints.FreezePositionZ;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
				if (FreezeRotationX)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezeRotationX;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
				if (FreezeRotationY)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezeRotationY;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
				if (FreezeRotationZ)
					thisRigidbody.constraints|= RigidbodyConstraints.FreezeRotationZ;
				else 
					thisRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;				
			
			}
		}
		
	}
	
	public void ResetRigidbodyVariables(){
		
		//reset rigidbody variables
		Mass=1f;
		Drag=0f;
		AngularDrag=0.05f;
		UseGravity=true;
		IsKinematic=false;
		Interpolation = RigidbodyInterpolation.None;
		CollisionDetection = CollisionDetectionMode.Discrete;
		FreezePositionX=false;
		FreezePositionY=false;
		FreezePositionZ=false;
		FreezeRotationX=false;
		FreezeRotationY=false;
		FreezeRotationZ=false;
		
		//apply changes
		SetRigidbodyVariables();
		
	}
	
	public void ApplyMeshRenderer(){
		
		MeshRenderer selfMeshRenderer=GetComponent<MeshRenderer>();
		bool selfMesherRendererCastShadows = selfMeshRenderer.castShadows;
		bool selfMesherRendererReceiveShadows = selfMeshRenderer.receiveShadows;
		Material[] selfMesherRendererSharedMaterials = selfMeshRenderer.sharedMaterials;
		bool selfMesherRendererUseLightProbes = selfMeshRenderer.useLightProbes;
		Transform selfMesherRendererLightProbeAnchor = selfMeshRenderer.probeAnchor;
			
		Debug.Log ("Apply MeshRenderer");
		
		foreach (Transform child in transform.Find ("_Alphabets")){
			MeshRenderer thisMeshRenderer = child.gameObject.GetComponent<MeshRenderer>();
			Debug.Log (selfMeshRenderer);
			if (thisMeshRenderer!=null){
				thisMeshRenderer.castShadows = selfMesherRendererCastShadows;
				thisMeshRenderer.receiveShadows = selfMesherRendererReceiveShadows;
				thisMeshRenderer.sharedMaterials = selfMesherRendererSharedMaterials;
				thisMeshRenderer.useLightProbes = selfMesherRendererUseLightProbes;
				thisMeshRenderer.probeAnchor = selfMesherRendererLightProbeAnchor;
			}
		}
		
		foreach (Transform child in transform){
			MeshRenderer thisMeshRenderer = child.gameObject.GetComponent<MeshRenderer>();
			if (thisMeshRenderer!=null){
				thisMeshRenderer.castShadows = selfMesherRendererCastShadows;
				thisMeshRenderer.receiveShadows = selfMesherRendererReceiveShadows;
				thisMeshRenderer.sharedMaterials = selfMesherRendererSharedMaterials;
				thisMeshRenderer.useLightProbes = selfMesherRendererUseLightProbes;
				thisMeshRenderer.probeAnchor = selfMesherRendererLightProbeAnchor;
			}
		}
		
		
	}
	
	public void DisableSelf(){
		
		enabled=false;
		//Debug.Log ("enabled? "+enabled);
		
	}
	
	public void EnableSelf(){
		
		enabled=true;
		//Debug.Log ("enabled? "+enabled);
		
	}
	
}
