using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio_Pieces : MonoBehaviour {

	public bool pickedUp;

	Collider col;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		pickedUp = false;
		col = GetComponentInChildren<Collider>();
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PickUpPutDown(bool pickUp, CharacterController cc)
	{
		col.isTrigger = pickUp;
		rb.isKinematic = pickUp;
		pickedUp = pickUp;
	}
}
