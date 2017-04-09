using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio_Pieces : MonoBehaviour, IInteractable {

	public bool pickedUp;

	MeshRenderer mesh;

	// Use this for initialization
	void Start () {
		pickedUp = false;
		mesh = GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string ActionDescription(){
		if (!pickedUp) {
			return "Press E to pickup radio piece";
		} else {
			return "";
		}
	}

	public void Action(){
		pickedUp = true;
		mesh.enabled = false;
	}
}
