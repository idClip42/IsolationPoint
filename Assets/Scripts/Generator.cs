using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public bool startLightsOn = true;

	GameObject[] allLights;

	void Start () 
	{
		allLights = GameObject.FindGameObjectsWithTag("ElectricLight");

		TurnLights(startLightsOn);
	}

	void TurnLights(bool on)
	{
		List <Material> materialList = new List<Material>();
		for(int n = 0; n < allLights.Length; ++n)
		{
			allLights[n].GetComponentInChildren<Light>().enabled = on;
			float intensity = 2.0f;
			if(!on)
				intensity = 0;
			allLights[n].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f, 1.0f) * intensity);
		}
	}
}
