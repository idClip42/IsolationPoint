using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour 
{
	Light light;
	Collider col;

	void Start () 
	{
		light = GetComponentInChildren<Light>();
		col = GetComponent<Collider>();
	}

	public void PickUpPutDown(bool pickUp)
	{
		col.isTrigger = pickUp;
		light.enabled = pickUp;
	}
}
