using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour 
{
	Light light;
	Collider col;
	Rigidbody rb;

	void Start () 
	{
		light = GetComponentInChildren<Light>();
		col = GetComponentInChildren<Collider>();
		rb = GetComponent<Rigidbody>();
	}

	public void PickUpPutDown(bool pickUp)
	{
		col.isTrigger = pickUp;
		light.enabled = pickUp;
		rb.isKinematic = pickUp;
	}
}
