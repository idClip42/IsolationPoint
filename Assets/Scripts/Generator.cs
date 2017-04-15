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
                if (objectiveToFixGenerator != null) objectiveToFixGenerator.IsCompleted = true;
                if (hasGas)
                {
                    SwitchLights(true);
                }
            }
        }
    }
	public bool currentlyFixing;
    public bool hasGas;
    public bool HasGas
    {
        get { return hasGas; }
        set {
            hasGas = value;
            if (hasGas && isFixed)
            {
                SwitchLights(true);
            }
        }
    }

    public Objective objectiveToFillWithGas;
    public Objective objectiveToFixGenerator;
    LightFlickerEvent flickerEvent;

    public float maxTime;
	float timer;

	const float MAX_TIME = 60.0f * 5.0f;

	public GameObject gasMonitor;

	void Start () 
	{
		allLights = GameObject.FindGameObjectsWithTag("ElectricLight");
		lightsOn = false;
		UIScript = GameObject.Find ("UI").GetComponent<UI_Manager>();
		isFixed = false;
		currentlyFixing = false;
        hasGas = true;
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

		UpdateMonitor((maxTime - timer)/maxTime);
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
        if (!isFixed && !currentlyFixing)
        {
            return "Press E to fix generator";
        }
        else if (currentlyFixing)
        {
            return "Generator being fixed";
        }
        else if (!hasGas)
        {
            return "Fill with gas";
        }
        else
        {
            return "Generator working properly";
        }
	}

	public void Action(){
        if (currentlyFixing || (isFixed && hasGas)) return;//only if fixed and running or being fixed
        //to fix generator
        if (!isFixed)
        {
            currentlyFixing = true;
            UIScript.StartGeneratorFix();
        }
        else//to fill generator with gas
        {
            GasCan gasCan = GetCurrentGasScript();
            if (gasCan.hasGas)
            {
                HasGas = true;
                gasCan.hasGas = false;//remove gas
                if (objectiveToFillWithGas != null) objectiveToFillWithGas.IsCompleted = true;
            }
        }
        
	}

	void UpdateTimer(){
        //only update if the lights are on and generator fixed
        if (!lightsOn || !isFixed) return;

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

	void UpdateMonitor(float percentFull)
	{
		if(gasMonitor == null) return;
		if(percentFull > 1.0f) percentFull = 1.0f;
		if(percentFull < 0) percentFull = 0;

		Material mat = gasMonitor.GetComponent<MeshRenderer>().material;
		mat.mainTextureOffset = new Vector2(0, 0.5f * percentFull);
		mat.SetColor("_EmissionColor", new Color((1.0f-percentFull), percentFull, 0, 1.0f));
	}
}
