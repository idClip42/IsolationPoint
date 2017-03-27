using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public bool startLightsOn = true;
	public bool debugLightsOnLKey = false;

	GameObject[] allLights;
	bool lightsOn;

	void Start () 
	{
		allLights = GameObject.FindGameObjectsWithTag("ElectricLight");
		lightsOn = false;

		SwitchLights(startLightsOn);
	}

	void Update ()
	{
		if(debugLightsOnLKey == true)
			if(Input.GetKeyDown(KeyCode.L))
				SwitchLights(!lightsOn);
	}

	public void SwitchLights(bool lOn)
	{
		lightsOn = lOn;
		List <Material> materialList = new List<Material>();
		for(int n = 0; n < allLights.Length; ++n)
		{
			allLights[n].GetComponentInChildren<Light>().enabled = lOn;
			float intensity = 2.0f;
			if(!lOn)
				intensity = 0;
			allLights[n].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f, 1.0f) * intensity);
		}
	}
}
