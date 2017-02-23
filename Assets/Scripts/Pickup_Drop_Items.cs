using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Drop_Items : MonoBehaviour {

	GameObject currentItem;

	Transform playerHand;
	Transform playerTransform;

	Vector3 dropLocation;

	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.R) && currentItem != null) {
			SetTransform ();
			DropItem ();
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			SetTransform ();
			if (CheckValidItem ()) {
				GetPlayerHand ();
				SetCurrentItem ();
			}
		}
	}

	/// <summary>
	/// Checks the distance between the player and the item raycasted
	/// </summary>
	/// <returns><c>true</c>, if distance was checked, <c>false</c> otherwise.</returns>
	bool CheckValidItem() {

		// creates ray at mouse position
		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		RaycastHit hit;

		// Raycasts from main camera forward vector and returns true if the item is within 2 units and has correct tag
		if (Physics.Raycast (Camera.main.transform.position, forward, out hit, 2)) {
			if(hit.transform.tag == "CanPickUp") {
				currentItem = hit.transform.gameObject;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Sets the current item to the hand transform and parents it to the hand
	/// </summary>
	void SetCurrentItem() {
		currentItem.transform.position = playerHand.position;
		currentItem.transform.parent = playerHand;
	}

	/// <summary>
	/// Gets the player's hand
	/// </summary>
	void GetPlayerHand() {
		Transform[] transforms = playerTransform.GetComponentsInChildren<Transform> ();
		foreach (Transform t in transforms) {
			if (t.gameObject.name == "Hand_R") {
				playerHand = t;
			}
		}
	}

	/// <summary>
	/// Raycasts below the player and drops the object slightly in front of him
	/// </summary>
	void DropItem(){
		currentItem.transform.parent = null;
		RaycastHit hit;

		if (Physics.Raycast (playerTransform.position, Vector3.down, out hit, 10)) {
			dropLocation = hit.point;
			dropLocation += (transform.forward * 0.5f);
			currentItem.transform.position = dropLocation;
		}

		currentItem = null;
	}

	/// <summary>
	/// Gets the current transform of the player
	/// </summary>
	void SetTransform() {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
	}
}
