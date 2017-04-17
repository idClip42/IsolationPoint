﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put other actions for other objects here?
/// </summary>
public class PlayerAction : MonoBehaviour {

    //contains the tag and associated script that holds the interaction action for the object
    Dictionary<string, System.Type> componentDict = new Dictionary<string, System.Type>();

    GameManager gm;
    Text text;

    public float callDistance = 5;
    //public float goToDistance = 15;

	// Use this for initialization
	void Start () {
        //add tag and associated script type to dictionary
        componentDict.Add("Door", typeof(DoorMovement));
		componentDict.Add("Timeskipper", typeof(DaySkip));
        componentDict.Add("Generator", typeof(Generator));
        componentDict.Add("GasSource", typeof(GasSource));
        componentDict.Add("UnlockDoor", typeof(UnlockDoorObject));
        componentDict.Add("RadioPiece", typeof(Radio_Pieces));
        componentDict.Add("Radio", typeof(Radio));

        gm = GameObject.Find("GM").GetComponent<GameManager>();

		GameObject ObjIntText = GameObject.Find("Object Interaction Text");
		if(ObjIntText != null)
			text = ObjIntText.GetComponent<Text>();
		else
			Debug.Log("Need 'Object Interaction Text' Object");
    }
	
	// Update is called once per frame
	void Update () {
        //reset ui text
		if(text != null)
        	text.text = " ";

        //check if input is allowed
        if (gm.PauseInput) return;

        //find the game object in front of current character
        GameObject obj = CheckForTag();
        if (obj != null)
        {
            //get script type
            System.Type t = componentDict[obj.tag];
            //check if interactable --> should always be but just in case
            if (obj.GetComponent(t) is IInteractable)
            {
                //set interaction text
                text.text = (obj.GetComponent(t) as IInteractable).ActionDescription();

                //check for button press
                if (Input.GetButtonDown("Action"))
                {
                    (obj.GetComponent(t) as IInteractable).Action();
                }
            }
        }

        //check for group key press
        if (Input.GetButtonDown("Group"))
        {
            GameObject currentPlayer = GetComponent<PlayerController>().Player.gameObject;
            foreach (GameObject player in gm.players)
            {
                if (player == currentPlayer) continue;
                if((player.transform.position - currentPlayer.transform.position).sqrMagnitude <= Mathf.Pow(callDistance, 2))
                {
                    //set navagent destination to current player
                    Follower f = player.GetComponent<Follower>();
                    f.Agent.enabled = true;
                    f.SetLeader(currentPlayer);
                }
            }
        }

        //stop following or go to target
        if (Input.GetButtonDown("Stay"))
        {
            GameObject currentPlayer = GetComponent<PlayerController>().Player.gameObject;
            foreach (GameObject player in gm.players)
            {
                Follower f = player.GetComponent<Follower>();
                if (player == currentPlayer)
                {
                    f.GoTo = false;
                }
                else
                {
                    //set navagent destination to current player
                    f.Stay();
                }
            }
        }

        //check for go to key press
        if (Input.GetButtonDown("GoTo"))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit);
            if (hit.transform == null) return;
            GameObject currentPlayer = GetComponent<PlayerController>().Player.gameObject;
            Follower f = currentPlayer.GetComponent<Follower>();
            f.SetDestination(hit.point);
        }
    }



    /// <summary>
    /// Check if the gameobject is tagged to have a script.
    /// </summary>
    /// <returns>Returns the object if it has a tag in the dictionary.</returns>
    GameObject CheckForTag()
    {
        // creates ray at mouse position
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		float aimRadius = 0.05f;	// radius of sphere cast - margin of error for aiming at objects

        // Raycasts from main camera forward vector and returns true if the item is within 2 units and has correct tag
		//if (Physics.Raycast(Camera.main.transform.position, forward, out hit, 5))
		if(Physics.SphereCast (Camera.main.transform.position, aimRadius, forward, out hit, 5, ~(0 << 8), QueryTriggerInteraction.Ignore))
        {
            foreach(string key in componentDict.Keys)
            {
                if(hit.transform.tag == key)
                {
                    return hit.transform.gameObject;
                }
            }
        }
        return null;
    }
}
