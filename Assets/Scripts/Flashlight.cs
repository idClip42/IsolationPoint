using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour 
{
	Light light;
	Collider col;
	Rigidbody rb;
	Camera cam;
	CharacterController currentChar;
	GameObject currentModel;
	GameObject flashightObj;
	Quaternion flashlightOrigPos;


	void Start () 
	{
		light = GetComponentInChildren<Light>();
		col = GetComponentInChildren<Collider>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main;
		currentChar = null;
		currentModel = null;
		flashightObj = col.gameObject;
		flashlightOrigPos = flashightObj.transform.rotation;
	}

	void Update()
	{
		if(currentChar != PlayerController.controller.Player) return;

		// Aims the flashlight where the camera is looking, within reason

		float angleLimit = 90.0f;
		float lerpVal = 0.5f;

		Vector3 charForward = currentModel.transform.forward;
		Vector3 camForward = cam.transform.forward;
		float angle = Vector3.Angle(charForward, camForward);
		if(angle < angleLimit)
			flashightObj.transform.up = Vector3.Lerp(		// Must be up because of way object is oriented
				flashightObj.transform.up,
				-camForward,
				lerpVal
			);
		else
			flashightObj.transform.rotation = Quaternion.Lerp(
				flashightObj.transform.rotation,
				flashlightOrigPos,
				lerpVal
			);
	}

	public void PickUpPutDown(bool pickUp, CharacterController c)
	{
		col.isTrigger = pickUp;
		light.enabled = pickUp;
		rb.isKinematic = pickUp;
		if(pickUp)
		{
			currentChar = c;
			currentModel = currentChar.GetComponentInChildren<Animator>().gameObject;
		} else {
			currentChar = null;
			currentModel = null;
		}
	}
}
