using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put other actions for other objects here?
/// </summary>
public class PlayerAction : MonoBehaviour {

    //contains the tag and associated script that holds the interaction action for the object
    Dictionary<string, System.Type> componentDict = new Dictionary<string, System.Type>();

    Text text;

	// Use this for initialization
	void Start () {
        //add tag and associated script type to dictionary
        componentDict.Add("Door", typeof(DoorMovement));
        componentDict.Add("Timeskipper", typeof(DaySkip));

        text = GameObject.Find("Object Interaction Text").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        //reset ui text
        text.text = " ";

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
    }



    /// <summary>
    /// Check if the gameobject is a door.
    /// </summary>
    /// <returns>Returns the object if it is a door.</returns>
    GameObject CheckForTag()
    {
        // creates ray at mouse position
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        // Raycasts from main camera forward vector and returns true if the item is within 2 units and has correct tag
        if (Physics.Raycast(Camera.main.transform.position, forward, out hit, 5))
        {
            /*
            if (hit.transform.tag == "Door")
            {
                return hit.transform.gameObject;
            }
            */
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
