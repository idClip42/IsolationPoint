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
    public bool IsFixed
    {
        get { return isFixed; }
        set
        {
            isFixed = value;
            if (isFixed)
            {
                SwitchLights(true);
            }
        }
    }
	public bool currentlyFixing;

    public Objective[] objectivesToAffect;
    LightFlickerEvent flickerEvent;

    public float maxTime;
	float timer;
    public bool startTimer = false;

	const float MAX_TIME = 60.0f * 5.0f;

	void Start () 
	{
		allLights = GameObject.FindGameObjectsWithTag("ElectricLight");
		lightsOn = false;
		UIScript = GameObject.Find ("UI").GetComponent<UI_Manager>();
		isFixed = false;
		currentlyFixing = false;
        flickerEvent = GetComponent<LightFlickerEvent>();
        if (flickerEvent)
        {
            flickerEvent.generator = this;
        }

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
            allLights[n].GetComponentInChildren<Light>().intensity = intensity;
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
        GasCan gasCan = GetCurrentGasScript();
        if (gasCan == null || !gasCan.hasGas) return;//need a gas can with gas
        if (currentlyFixing || isFixed) return;//only if not fixed or being fixed

		currentlyFixing = true;
        gasCan.hasGas = false; //remove gas

        foreach(Objective o in objectivesToAffect)
        {
            o.IsCompleted = true;
        }

		UIScript.StartGeneratorFix();
	}

	void UpdateTimer(){
        //only update if the lights are on
        if (!lightsOn || !startTimer) return;

		timer += Time.deltaTime;

		if (timer > maxTime) {
            if (flickerEvent != null)
            {
                //play the flicker event -- will control lightsOn
                flickerEvent.PlayEvent();
                //By the time event completes and changes(or not) lightsOn timer should be 0ish again
                timer = -flickerEvent.timeToComplete;
            }
		}
	}

    GasCan GetCurrentGasScript()
    {
        CharacterController currentPlayer = PlayerController.controller.Player;
        Pickup_Drop_Items pickupScript = currentPlayer.GetComponent<Pickup_Drop_Items>();
        GameObject item = pickupScript.LeftHandItem;
        if (item == null) return null;
        GasCan gasScript = item.GetComponent<GasCan>();
        return gasScript;
    }
}
