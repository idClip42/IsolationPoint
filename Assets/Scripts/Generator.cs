using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	GameObject[] allLights;

	void Start () 
	{
		allLights = GameObject.FindGameObjectsWithTag("ElectricLight");

		TurnLights(true);
	}

	void TurnLights(bool on)
	{
		for(int n = 0; n < allLights.Length; ++n)
		{
			allLights[n].GetComponent<Light>().enabled = on;
		}
	}
}
