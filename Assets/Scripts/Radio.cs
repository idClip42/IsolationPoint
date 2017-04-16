using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IInteractable {

	UI_Manager UIScript;

	public bool isFixed;
	public bool isFixing;

	Radio_Pieces[] radioPieces;

	int fixedPieces;

	const int TOTAL_PIECES = 4;

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
		if (fixedPieces < TOTAL_PIECES) {
			return "Find all pieces to repair radio " + fixedPieces + "/4";
		}
		if (fixedPieces == TOTAL_PIECES && isFixed == false) {
			return "All pieces found, press E to fix radio";
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
		if (fixedPieces == TOTAL_PIECES) {
			UIScript.StartRadioFix ();
		}

		foreach (Radio_Pieces piece in radioPieces) {
			if (piece.pickedUp == true) {
				fixedPieces++;
				piece.pickedUp = false;
				Destroy (piece.gameObject);
			}
		}
	}
}
