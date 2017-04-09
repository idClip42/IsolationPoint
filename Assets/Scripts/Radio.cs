using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IInteractable {

	UI_Manager UIScript;

	public bool isFixed;
	public bool isFixing;

	Radio_Pieces[] radioPieces;

	// Use this for initialization
	void Start () {
		UIScript = GameObject.Find ("UI").GetComponent<UI_Manager>();
		isFixed = false;
		isFixing = false;
		radioPieces = GameObject.Find ("RadioPieces").GetComponentsInChildren<Radio_Pieces> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string ActionDescription(){
		foreach (Radio_Pieces piece in radioPieces) {
			if (piece.pickedUp == false) {
				return "Find all pieces to repair the radio";
			}
		}
		if (isFixing) {
			return "Radio is being fixed";
		}
		if (isFixed) {
			return "Radio working properly!";
		} else {
			return "Press E to repair radio";
		}
	}

	public void Action(){
		foreach (Radio_Pieces piece in radioPieces) {
			if (piece.pickedUp == false) {
				return;
			}
		}

		UIScript.StartRadioFix ();
	}
}
