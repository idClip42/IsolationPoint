using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Drop_Items : MonoBehaviour {

	GameObject currentItem;

	Transform playerHand;
	Transform playerTransform;

	Vector3 dropLocation;

	Combat combatScript;

	Quaternion bladeRotation;

	PlayerController playerController;

	void Start () {
		bladeRotation = new Quaternion (0, 45f, 0, 0);
		//playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		playerTransform = transform;
		playerController = GameObject.Find ("PlayerController").GetComponent<PlayerController> ();
		combatScript = GetComponent<Combat> ();
	}
	
	void Update () {

		if(playerController.Player.gameObject != this.gameObject) return;

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

		Debug.DrawRay (Camera.main.transform.position, Camera.main.transform.forward * 2, Color.green);
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
		if (Physics.Raycast (Camera.main.transform.position, forward, out hit, 5)) {
			if(Vector3.Distance(playerTransform.position, hit.transform.position) <= 2 && hit.transform.tag == "Weapon") {
				currentItem = hit.transform.gameObject;
				combatScript.PickUpWeapon (currentItem);
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
		currentItem.transform.localPosition = Vector3.zero;
		currentItem.transform.localRotation = Quaternion.identity;
		if (currentItem.name.Contains("WoodSword"))
			currentItem.transform.Rotate(0, -90, 0);
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

		combatScript.PickUpWeapon (null);

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
		playerTransform = playerController.Player.GetComponent<Transform> ();
	}

	public Transform PlayerTransform{
		get { return playerTransform;}
		set { playerTransform = value;}
	}
}
