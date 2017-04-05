using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour, IInteractable {

	UI_Manager UIScript;

	public bool startLightsOn = true;
	public bool debugLightsOnLKey = false;

	GameObject[] allLights;
    public GameObject[] AllLights
    {
        get { return allLights; }
    }
	bool lightsOn;
	public bool isFixed;
	public bool currentlyFixing;

	float timer;

	const float MAX_TIME = 60.0f * 5.0f;

	void Start () 
	{
		allLights = GameObject.FindGameObjectsWithTag("ElectricLight");
		lightsOn = false;
		UIScript = GameObject.Find ("UI").GetComponent<UI_Manager>();
		isFixed = false;
		currentlyFixing = false;

		SwitchLights(startLightsOn);
	}

	void Update ()
	{
		if(debugLightsOnLKey == true)
			if(Input.GetKeyDown(KeyCode.L))
				SwitchLights(!lightsOn);

		UpdateTimer ();
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

	public string ActionDescription(){
		if (!isFixed && !currentlyFixing) {
			return "Press E to fix generator";
		} else if (currentlyFixing) {
			return "Generator being fixed";		
		} else {
			return "Generator working properly";
		}
	}

	public void Action(){
		currentlyFixing = true;

		UIScript.ShowGBar ();
	}

	void UpdateTimer(){
		timer += Time.deltaTime;

		if (timer > 5.0f) {
			timer = 0.0f;
		}
	}
}
