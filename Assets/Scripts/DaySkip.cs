using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySkip : MonoBehaviour, IInteractable {

    GameObject sun;
    bool isUsed = false;

	// Use this for initialization
	void Start () {
        sun = GameObject.Find("Sun");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Action()
    {
        if (!isUsed)
        {
            isUsed = true;
            sun.GetComponent<SunSetting>().NightFalls();
        }
    }

    public string ActionDescription()
    {
        if (!isUsed)
        {
            return "End day";
        }
        return "";
    }
}
