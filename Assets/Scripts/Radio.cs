﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour, IInteractable {

	UI_Manager UIScript;

	public bool isFixed;
	public bool isFixing;

	Radio_Pieces[] radioPieces;

	int fixedPieces;

	const int TOTAL_PIECES = 4;

    public Objective onFixObj;  //Actually gets completed in UI...

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

        Radio_Pieces rPiece = GetCurrentRadioPiece();
        if (rPiece == null) return;

		foreach (Radio_Pieces piece in radioPieces) {
			if (piece == rPiece) {//only use the one in the character's hand
				fixedPieces++;
				piece.pickedUp = false;
				Destroy (piece.gameObject);
			}
		}
	}

    Radio_Pieces GetCurrentRadioPiece()
    {
        CharacterController currentPlayer = PlayerController.controller.Player;
        Pickup_Drop_Items pickupScript = currentPlayer.GetComponent<Pickup_Drop_Items>();
        GameObject item = pickupScript.LeftHandItem;
        if (item == null) return null;
        Radio_Pieces piece = item.GetComponent<Radio_Pieces>();
        return piece;
    }
}
