using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Drop_Items : MonoBehaviour {

	GameObject currentItem;
	GameObject droppedItem;
	Transform playerHand;
	Transform playerTransform;
	PlayerController playerController;
	float playerRadius;

	// Use this for initialization
	void Start () {
		playerController = GameObject.Find ("PlayerController").GetComponent<PlayerController> ();
		playerTransform = GameObject.Find ("PlayerController").GetComponent<Transform> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			if (CheckValidItem ()) {
				GetPlayerHand ();
				SetCurrentItem ();
				// set current item as the raycasted item
			}
		}
	}

	/// <summary>
	/// Checks the distance between the player and the item raycasted
	/// </summary>
	/// <returns><c>true</c>, if distance was checked, <c>false</c> otherwise.</returns>
	bool CheckValidItem() {

		// creates ray at mouse position
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		// Raycasts onto the mouse position and returns true if the item is within 2 units and has correct tag
		if (Physics.Raycast (ray, out hit)) {
			if(Vector3.Distance(playerTransform.position, hit.transform.position) <= 2 && hit.transform.tag == "CanPickUp") {
				currentItem = hit.transform.gameObject;
				return true;
			}
		}
		return false;	
	}

	void SetCurrentItem() {
		currentItem.transform.position = playerHand.position;
		currentItem.transform.parent = playerHand;
	}

	void GetPlayerHand() {
		Transform[] transforms = playerController.Player.GetComponentsInChildren<Transform> ();
		Debug.Log ("Called!");
		foreach (Transform t in transforms) {
			if (t.gameObject.name == "Hand_R") {
				playerHand = t;
				Debug.Log (playerHand.name);
			}
		}
	}
}
